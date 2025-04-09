using System.Collections.Generic;

public static class TagManager
{
	private static readonly Dictionary<TagType, string> _tags;

	static TagManager()
	{
		_tags = new Dictionary<TagType, string>
			{
				{TagType.Player, "Player"},
				{TagType.DragableObject, "DragableObject"},
				{TagType.Wall, "Wall"},
			};
	}

	public static string GetTag(TagType tagType)
	{
		return _tags[tagType];
	}
}
