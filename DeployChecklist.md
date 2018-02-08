# SpectNetIde Deployment Check List

1. The entire soution builds with the Release configuration (__Clean Solution__, __Rebuil Solution__)
2. All unit tests run successfully (no failed or ignored test cases)
3. __`AssemblyVersion`__ and __`AssemblyFileVersion`__ attributes are updated in these solutions:
    * __`Spect.Net.CodeDiscover.ProjectType`__
    * __`Spect.Net.ProjectWizard`__
    * __`Spect.Net.VsPackage`__
 4. Version numbers use semantic versioning:
    * __Major version__: New features even with potential breaking changes
    * __Minor version__: New features while keeping compatibility with previous versions
    * __Build version__: Patches, hotfixes, minor corrections
    * __Revision number__: Cosmetic changes
5. VSIX manifes info (new version and other potential changes) updated in the source.extension.vsixmanifest files of these projects:
    * __`Spect.Net.CodeDiscover.ProjectType`__
    * __`Spect.Net.VsPackage`__