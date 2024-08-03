namespace SonicNextModManager.SiS
{
    public class SprintConverter
    {
        public void Convert(string file)
        {
            using (StreamReader streamReader = new(file))
            {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    // TODO: https://github.com/hyperbx/SonicNextModManager/blob/Project-Rush/Sonic-06-Mod-Manager/src/UnifyPatcher.cs#L392
                }
            }
        }
    }
}
