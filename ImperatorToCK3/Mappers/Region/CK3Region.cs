﻿using commonItems;
using System.Collections.Generic;

namespace ImperatorToCK3.Mappers.Region {
	public class CK3Region {
		public Dictionary<string, CK3Region?> Regions { get; private set; } = new();
		public Dictionary<string, CK3.Titles.Title?> Duchies { get; private set; } = new();
		public Dictionary<string, CK3.Titles.Title?> Counties { get; private set; } = new();
		public SortedSet<ulong> Provinces { get; private set; } = new();

		public void LinkRegion(string regionName, CK3Region region) {
			Regions[regionName] = region;
		}
		public void LinkDuchy(CK3.Titles.Title theDuchy) {
			Duchies[theDuchy.Id] = theDuchy;
		}
		public void LinkCounty(CK3.Titles.Title theCounty) {
			Counties[theCounty.Id] = theCounty;
		}
		public bool ContainsProvince(ulong provinceId) {
			foreach (var region in Regions.Values) {
				if (region?.ContainsProvince(provinceId) == true) {
					return true;
				}
			}
			foreach (var duchy in Duchies.Values) {
				if (duchy?.DuchyContainsProvince(provinceId) == true) {
					return true;
				}
			}
			foreach (var county in Counties.Values) {
				if (county?.CountyProvinces.Contains(provinceId) == true) {
					return true;
				}
			}
			return Provinces.Contains(provinceId);
		}

		private static readonly Parser parser = new();
		private static CK3Region regionToReturn = new();
		static CK3Region() {
			parser.RegisterKeyword("regions", reader => {
				foreach (var name in new StringList(reader).Strings) {
					regionToReturn.Regions.Add(name, null);
				}
			});
			parser.RegisterKeyword("duchies", reader => {
				foreach (var name in new StringList(reader).Strings) {
					regionToReturn.Duchies.Add(name, null);
				}
			});
			parser.RegisterKeyword("counties", reader => {
				foreach (var name in new StringList(reader).Strings) {
					regionToReturn.Counties.Add(name, null);
				}
			});
			parser.RegisterKeyword("provinces", reader => {
				foreach (var id in new ULongList(reader).ULongs) {
					regionToReturn.Provinces.Add(id);
				}
			});
			parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
		}
		public static CK3Region Parse(BufferedReader reader) {
			regionToReturn = new CK3Region();
			parser.ParseStream(reader);
			return regionToReturn;
		}
	}
}
