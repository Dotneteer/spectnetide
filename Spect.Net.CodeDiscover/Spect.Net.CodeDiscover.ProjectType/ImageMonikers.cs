using System;
using Microsoft.VisualStudio.Imaging.Interop;

namespace ZXSpectrumCodeDiscover
{
    public static class ImageMonikers
    {
        private static readonly Guid s_ManifestGuid = new Guid("a9d5b7d1-2f7d-45c2-b8cf-9205db07de5b");

        private const int PROJECT_ICON = 0;
        private const int DISASSANN_ICON = 1;
        private const int ROM_ICON = 2;

        public static ImageMoniker ProjectIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = PROJECT_ICON };
        public static ImageMoniker DisassAnnIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = DISASSANN_ICON };
        public static ImageMoniker RomIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = ROM_ICON };
    }
}
