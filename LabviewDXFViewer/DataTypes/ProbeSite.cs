using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabviewDXFViewer.DataTypes
{
    public class ProbeSite
    {
        public string JunctionName { get; set; }
        public string Orientation { get; set; }
        public string Function { get; set; }
        public Point Position { get; set; }

        public double Area { get; set; }
        public double TopWidth { get; set; }
        public double BottomWidth { get; set; }
        public ProbeSite() { }
        public ProbeSite(string position)
        {
            var parts = position.Trim().Split(',');
            Position = new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }

    public enum ProbeOrientation:int
    {
        Horizontal=0, Vertical=1
    }
    public enum ProbeFunction:int
    {
        IV=0,
        IVC=1,
        Breakdown=2, 
        Joule=3, 
        [Description("dC/dV")]
        dCdV=4,
        [Description("C/F")]
        CF =5,
        Leakage =6,
        LeakageThreshold=7
    }


    public static class EnumHelper
    {
        public static string ToDescription(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!Enum.IsDefined(value.GetType(), value))
            {
                return string.Empty;
            }

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo != null)
            {
                DescriptionAttribute[] attributes =
                    fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return StringHelper.ToFriendlyName(value.ToString());
        }
    }

    public static class StringHelper
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || string.IsNullOrEmpty(value.Trim());
        }

        public static string ToFriendlyName(string value)
        {
            if (value == null) return string.Empty;
            if (value.Trim().Length == 0) return string.Empty;

            string result = value;

            result = string.Concat(result.Substring(0, 1).ToUpperInvariant(), result.Substring(1, result.Length - 1));

            const string pattern = @"([A-Z]+(?![a-z])|\d+|[A-Z][a-z]+|(?![A-Z])[a-z]+)+";

            List<string> words = new List<string>();
            Match match = Regex.Match(result, pattern);
            if (match.Success)
            {
                Group group = match.Groups[1];
                foreach (Capture capture in group.Captures)
                {
                    words.Add(capture.Value);
                }
            }

            return string.Join(" ", words.ToArray());
        }
    }
}
