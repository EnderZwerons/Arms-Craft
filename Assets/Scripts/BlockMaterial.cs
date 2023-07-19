using System;

[Serializable]
public class BlockMaterial
{
	public BlockMaterialInfo[] blockMaterialInfo;

	public int buyPrice;

	public int createPrice;

	public long createTime;

	public long CreateTime
	{
		get
		{
			//screw you!
			return 0;
		}
	}
}
