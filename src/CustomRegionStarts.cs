using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mehr1us.expedition
{
    public class CustomRegionStarts
    {
        private Regex pattern = new(@"^\w{2,3}[ \t]*(\[\d\]+)?[ \t]*:[ \t]*(X-)?(\w+[ \t]*,?[ \t]*)+", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        public List<Rule> ruleset = new();
        public Dictionary<string, int> customRegionWeight = new();

        public void MergeRegionLines(List<string> regionsLines)
        {
            foreach (string line in regionsLines)
            {
                if (pattern.IsMatch(line.Trim()))
                {
                    String[] split = line.Split(':');
                    String region = split[0].Split('[')[0].Trim();
                    if (split[0].Contains(']'))
                    {
                        int regionWeight = int.Parse(split[0].Trim().Split('[')[1].Split(']')[0]);
                        customRegionWeight[region] = regionWeight;
                    }
                    String[] ids = split[1].Trim().Split(',');
                    bool exclude = ids[0].Trim().ToLower().StartsWith("x-");
                    if (exclude)
                    {
                        ids[0] = ids[0].Split('-')[1];
                    }
                    AddRule(exclude, region, ids);
                }
                
            }
        }

        private void AddRule(bool exclude, string region, string[] ids)
        {
            if (!ruleset.TrueForAll(x => x.region != region))
            {
                for (int i = 0; i < ruleset.Count; i++)
                {
                    if (ruleset[i].region == region)
                    {
                        List<String> existingIds = new(ruleset[i].ids);
                        foreach(string id in ids)
                        {
                            bool sameAction = exclude == ruleset[i].exclusion;
                            if (sameAction && !existingIds.Contains(id)) existingIds.Add(id);
                            else if (existingIds.Contains(id) && !ruleset[i].exclusion && !sameAction) existingIds.Remove(id);
                        }
                        ruleset[i].ids = existingIds.ToArray();
                        return;
                    }
                }
            }
            ruleset.Add(new Rule(exclude, region, ids));
        }

        public bool Exec(string region, SlugcatStats.Name scug)
        {
            foreach (Rule rule in ruleset)
            {
                if (rule.region.ToLower().Equals(region.ToLower()))
                {
                    bool ret = rule.Exec(scug.ToString());
                    Plugin.LogDebug(region + ((ret)? " is availible for" : " isn't availible for") + " " + scug);
                    return ret;
                }
            }
            // if there are no rules, presume that it is availible
            return false;
        }

        public override string ToString()
        {
            String outStr = "CustomRegionStarts {\n\t";
            foreach (Rule rule in ruleset)
            {
                outStr += rule.ToString() + "\n\t";
            }
            outStr += "Dictionary customRegionWeight : (keys: " + customRegionWeight.Keys.Count + ")";
            return outStr;
        }

        public class Rule
        {
            public bool exclusion;
            public String region;
            public String[] ids;

            public Rule(bool exclusion, string region, string[] ids)
            {
                this.exclusion = exclusion;
                this.region = region;
                this.ids = ids;
                for (int i = 0; i < this.ids.Length; i++)
                {
                    this.ids[i] = this.ids[i].Trim().ToLower();
                }
            }

            public bool Exec(string scug)
            {
                if (ids.Contains(scug.ToLower()))
                {
                    return !exclusion;
                }
                return exclusion;
            }
            public override string ToString()
            {
                string outStr = ((exclusion) ? "exclude: " : "include: ");
                for (int i = 0; i < ids.Length; i++)
                {
                    outStr += ids[i] + ((i < ids.Length - 1)? ", " : "");
                }
                return outStr;
            }
        }
    }
}
