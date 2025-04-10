using System.Collections.Generic;

public class TagManager
{
	public readonly Dictionary<TagType, string> _tags;


	public TagManager()
	{
		_tags = new Dictionary<TagType, string>
			{
				{TagType.Player, "Player"},
				{TagType.DragableObject, "DragableObject"},
				{TagType.Wall, "Wall"},
			};
	}

	public string GetTag(TagType tagType)
	{
		return _tags[tagType];
	}
}
