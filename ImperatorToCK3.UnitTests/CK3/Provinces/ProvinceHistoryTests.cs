﻿using commonItems;
using ImperatorToCK3.CK3.Provinces;
using Xunit;

namespace ImperatorToCK3.UnitTests.CK3.Provinces; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class ProvinceHistoryTests {
	private readonly Date ck3BookmarkDate = new(867, 1, 1);
	[Fact]
	public void FieldsDefaultToCorrectValues() {
		var province = new Province(1);
		Assert.Null(province.GetCultureId(ck3BookmarkDate));
		Assert.Null(province.GetFaithId(ck3BookmarkDate));
		Assert.Equal("none", province.GetHoldingType(ck3BookmarkDate));
		Assert.Empty(province.GetBuildings(ck3BookmarkDate));
	}

	[Fact]
	public void DetailsCanBeLoadedFromStream() {
		var reader = new BufferedReader(
			"= { religion = orthodox\n random_param = random_stuff\n culture = roman\n}"
		);
		var province = new Province(1, reader);

		Assert.Equal("roman", province.GetCultureId(ck3BookmarkDate));
		Assert.Equal("orthodox",province.GetFaithId(ck3BookmarkDate));
	}

	[Fact]
	public void DetailsAreLoadedFromDatedBlocks() {
		var reader = new BufferedReader(
			"= {" +
			"religion = catholic\n" +
			"random_param = random_stuff\n" +
			"culture = roman\n" +
			"850.1.1 = { religion=orthodox holding=castle_holding }" +
			"}"
		);
		var province = new Province(1, reader);

		Assert.Equal("castle_holding", province.GetHoldingType(ck3BookmarkDate));
		Assert.Equal("orthodox", province.GetFaithId(ck3BookmarkDate));
	}

	[Fact]
	public void DetailsCanBeCopyConstructed() {
		var reader = new BufferedReader(
			"= {" +
			"\treligion = catholic\n" +
			"\tculture = roman\n" +
			"\tbuildings = { orchard tavern }" +
			"\t850.1.1 = { religion=orthodox holding=castle_holding }" +
			"}"
		);
		var province1 = new Province(1, reader);
		var province2 = new Province(2, province1);

		Assert.Equal("castle_holding", province2.GetHoldingType(ck3BookmarkDate));
		Assert.Equal("orthodox", province2.GetFaithId(ck3BookmarkDate));
		Assert.Equal("roman", province2.GetCultureId(ck3BookmarkDate));
		Assert.Collection(province2.GetBuildings(ck3BookmarkDate),
			item1 => Assert.Equal("orchard", item1),
			item2 => Assert.Equal("tavern", item2)
		);
	}
}