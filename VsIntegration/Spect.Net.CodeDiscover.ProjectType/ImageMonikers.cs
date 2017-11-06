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
        private const int TZX_ICON = 3;
        private const int VMSTATE_ICON = 4;
        private const int Z80_ASM_ICON = 5;
        private const int TAP_ICON = 6;
        private const int TAPE_FOLDER = 7;
        private const int Z80_FOLDER = 8;

        public static ImageMoniker ProjectIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = PROJECT_ICON };
        public static ImageMoniker DisassAnnIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = DISASSANN_ICON };
        public static ImageMoniker RomIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = ROM_ICON };
        public static ImageMoniker TzxIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = TZX_ICON };
        public static ImageMoniker TapIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = TAP_ICON };
        public static ImageMoniker VmStateIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = VMSTATE_ICON };
        public static ImageMoniker Z80AsmIconImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = Z80_ASM_ICON };
        public static ImageMoniker TapeFolderImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = TAPE_FOLDER };
        public static ImageMoniker Z80FolderImageMoniker => new ImageMoniker { Guid = s_ManifestGuid, Id = Z80_FOLDER };
    }
}
