namespace ICSharpCode.SharpZipLib.Checksums
{
	public interface IChecksum
	{
		long Value { get; }

		void Reset();

		void Update(int value);
	}
}
