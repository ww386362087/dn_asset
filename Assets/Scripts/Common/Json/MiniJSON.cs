/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniJSON {
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }
    
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List<object>, a Dictionary<string, object>, a double, an integer,a string, null, true, or false</returns>
        //反序列化
        public static object Deserialize(string json) {
            // save the string for debug information
            if (json == null) {
                return null;
            }
            
            return Parser.Parse(json);
        }
        //阻止其他类从该类继承
        sealed class Parser : IDisposable 
        {
            const string WORD_BREAK = "{}[],:\"";
            
            public static bool IsWordBreak(char c) {
                //     如果 c 是空白，则为 true；否则，为 false;报告指定 Unicode 字符在此字符串中的第一个匹配项的索引。
                return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }
            
            enum TOKEN {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            };
            //     实现从字符串进行读取的 System.IO.TextReader。
            StringReader json;
            
            Parser(string jsonString) 
            {
                json = new StringReader(jsonString);
            }
            
            public static object Parse(string jsonString) 
            {
                using (var instance = new Parser(jsonString)) 
                {
                    return instance.ParseValue();
                }
            }
            //释放
            public void Dispose() {
                json.Dispose();
                json = null;
            }
            
            Dictionary<string, object> ParseObject() {
                Dictionary<string, object> table = new Dictionary<string, object>();
                
                // ditch opening brace
                json.Read();
                
                // {
                while (true) 
                {
                    switch (NextToken) 
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            // name
                            string name = ParseString();
                            if (name == null) 
                            {
                                return null;
                            }
                            
                            // :
                            if (NextToken != TOKEN.COLON) 
                            {
                                return null;
                            }
                            // ditch the colon
                            json.Read();
                            
                            // value
                            table[name] = ParseValue();
                            break;
                    }
                }
            }
            
            List<object> ParseArray() {
                List<object> array = new List<object>();
                
                // ditch opening bracket
                json.Read();
                
                // [
                bool parsing = true;
                while (parsing) {
                    TOKEN nextToken = NextToken;
                    
                    switch (nextToken) {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            object value = ParseByToken(nextToken);
                            
                            array.Add(value);
                            break;
                    }
                }
                
                return array;
            }
            
            object ParseValue() {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }
            
            object ParseByToken(TOKEN token) {
                switch (token) {
                    case TOKEN.STRING:
                        return ParseString();
                    case TOKEN.NUMBER:
                        return ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return ParseObject();
                    case TOKEN.SQUARED_OPEN:
                        return ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }
            
            string ParseString() {
                StringBuilder s = new StringBuilder();
                char c;
                
                // ditch opening quote
                json.Read();
                
                bool parsing = true;
                while (parsing) {
                    
                    if (json.Peek() == -1) {
                        parsing = false;
                        break;
                    }
                    
                    c = NextChar;
                    switch (c) {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (json.Peek() == -1) {
                                parsing = false;
                                break;
                            }
                            
                            c = NextChar;
                            switch (c) {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new char[4];
                                    
                                    for (int i=0; i< 4; i++) {
                                        hex[i] = NextChar;
                                    }
                                    
                                    s.Append((char) Convert.ToInt32(new string(hex), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }
                
                return s.ToString();
            }
            
            object ParseNumber() 
            {
                string number = NextWord;
                // 摘要:
                //     报告指定 Unicode 字符在此字符串中的第一个匹配项的索引。
                //
                // 参数:
                //   value:
                //     要查找的 Unicode 字符。
                //
                // 返回结果:
                //     如果找到该字符，则为 value 的从零开始的索引位置；如果未找到，则为 -1。
                if (number.IndexOf('.') == -1) 
                {
                    long parsedInt;
                    //     将数字的字符串表示形式转换为它的等效 64 位有符号整数。一个指示转换是否成功的返回值。
                    Int64.TryParse(number, out parsedInt);
                    return parsedInt;
                }
                
                double parsedDouble;
                Double.TryParse(number, out parsedDouble);
                return parsedDouble;
            }
            //
            void EatWhitespace() 
            {
                //指示指定字符串中位于指定位置处的字符是否属于空白类别。
                while (Char.IsWhiteSpace(PeekChar)) 
                {
                    json.Read();
                    //摘要:
                    //     返回下一个可用的字符，但不使用它。
                    //
                    // 返回结果:
                    //     表示下一个要读取的字符的整数，或者，如果没有更多的可用字符或该流不支持查找，则为 -1。
                    if (json.Peek() == -1) 
                    {
                        break;
                    }
                }
            }
            
            char PeekChar {
                get {
                    //     读取输入字符串中的下一个字符并将该字符的位置提升一个字符。
                    //
                    // 返回结果:
                    //     基础字符串中的下一个字符，或者如果没有更多的可用字符，则为 -1。
                    return Convert.ToChar(json.Peek());
                }
            }
            
            char NextChar {
                get {
                    return Convert.ToChar(json.Read());
                }
            }
            
            string NextWord {
                get {
                    //     表示可变字符字符串。无法继承此类。
                    StringBuilder word = new StringBuilder();
                    
                    while (!IsWordBreak(PeekChar)) {
                        // 摘要:
                        //     在此实例的结尾追加指定 Unicode 字符的字符串表示形式。
                        //
                        // 参数:
                        //   value:
                        //     要追加的 Unicode 字符。
                        //
                        // 返回结果:
                        //     完成追加操作后对此实例的引用。
                        word.Append(NextChar);
                        //下一个字符为空
                        if (json.Peek() == -1) {
                            break;
                        }
                    }
                    //
                    return word.ToString();
                }
            }
            
            TOKEN NextToken {
                get {
                    EatWhitespace();
                    
                    if (json.Peek() == -1) {
                        return TOKEN.NONE;
                    }
                    
                    switch (PeekChar) {
                        case '{':
                            return TOKEN.CURLY_OPEN;
                        case '}':
                            json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[':
                            return TOKEN.SQUARED_OPEN;
                        case ']':
                            json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            json.Read();
                            return TOKEN.COMMA;
                        case '"':
                            return TOKEN.STRING;
                        case ':':
                            return TOKEN.COLON;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }
                    
                    switch (NextWord) {
                        case "false":
                            return TOKEN.FALSE;
                        case "true":
                            return TOKEN.TRUE;
                        case "null":
                            return TOKEN.NULL;
                    }
                    
                    return TOKEN.NONE;
                }
            }
        }
        
        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary<string, object> / List<object></param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj) {
            return Serializer.Serialize(obj);
        }
        
        sealed class Serializer 
        {
            StringBuilder builder;
            
            Serializer() 
            {
                //创建生成器
                builder = new StringBuilder();
            }
            //序列化
            public static string Serialize(object obj) 
            {
                var instance = new Serializer();
                
                instance.SerializeValue(obj);
                
                return instance.builder.ToString();
            }
            //类型
            void SerializeValue(object value) 
            {
                IList asList;
                IDictionary asDict;
                string asStr;
                
                if (value == null) {
                    builder.Append("null");
                } else if ((asStr = value as string) != null) {
                    SerializeString(asStr);
                } else if (value is bool) {
                    builder.Append((bool) value ? "true" : "false");
                } else if ((asList = value as IList) != null) {
                    SerializeArray(asList);
                } else if ((asDict = value as IDictionary) != null) {
                    SerializeObject(asDict);
                } else if (value is char) {
                    SerializeString(new string((char) value, 1));
                } else {
                    SerializeOther(value);
                }
            }
            //序列化对象
            void SerializeObject(IDictionary obj) {
                bool first = true;
                
                builder.Append('{');
                
                foreach (object e in obj.Keys) {
                    if (!first) {
                        builder.Append(',');
                    }
                    
                    SerializeString(e.ToString());
                    builder.Append(':');
                    
                    SerializeValue(obj[e]);
                    
                    first = false;
                }
                
                builder.Append('}');
            }
            // 序列化数组
            void SerializeArray(IList anArray) {
                builder.Append('[');
                
                bool first = true;
                
                foreach (object obj in anArray) {
                    if (!first) {
                        builder.Append(',');
                    }
                    
                    SerializeValue(obj);
                    
                    first = false;
                }
                
                builder.Append(']');
            }
            //string
            void SerializeString(string str) {
                builder.Append('\"');
                
                char[] charArray = str.ToCharArray();
                foreach (var c in charArray) {
                    switch (c) {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126)) {
                                builder.Append(c);
                            } else {
                                builder.Append("\\u");
                                builder.Append(codepoint.ToString("x4"));
                            }
                            break;
                    }
                }
                
                builder.Append('\"');
            }
            //其他
            void SerializeOther(object value) {
                // NOTE: decimals lose precision during serialization.
                // They always have, I'm just letting you know.
                // Previously floats and doubles lost precision too.
                //注意：小数在序列化过程中丢失精度。
                //他们总是有，我只是让你知道。
                //以前失去精度和双精度浮点数。
                if (value is float) {
                    builder.Append(((float) value).ToString("R"));
                } else if (value is int
                           || value is uint
                           || value is long
                           || value is sbyte
                           || value is byte
                           || value is short
                           || value is ushort
                           || value is ulong) {
                    builder.Append(value);
                } else if (value is double
                           || value is decimal) {
                    builder.Append(Convert.ToDouble(value).ToString("R"));
                } else {
                    SerializeString(value.ToString());
                }
            }
        }
    }
}
