# SpectNetIde Deployment Check List

1. __`AssemblyVersion`__ and __`AssemblyFileVersion`__ attributes are updated in these solutions:
    * __`Spect.Net.CodeDiscover.ProjectType`__
    * __`Spect.Net.ProjectWizard`__
    * __`Spect.Net.VsPackage`__
 1. Version numbers use semantic versioning:
    * __Major version__: New features even with potential breaking changes
    * __Minor version__: New features while keeping compatibility with previous versions
    * __Build version__: Patches, hotfixes, minor corrections
    * __Revision number__: Cosmetic changes
1. VSIX manifest info (new version and other potential changes) updated in the source.extension.vsixmanifest files of these projects:
    * __`Spect.Net.CodeDiscover.ProjectType`__
    * __`Spect.Net.VsPackage`__
1. Check that all source files within the `Spect.Net.CodeDiscover.ProjectType` project's
`BuildSystem\DeployedBuildSystem` and `BuildSystem\Rules` folder are linked into the
`Spect.Net.VsPackage` project's `DeploymentResources` folder. The __Build Action__ property 
of these items should be __Embedded Resource__.
1. The `CURRENT_CPS_VERSION` value in `SpectNetPackage.cs` (`Spect.Net.VsPackage project`) is updated 
to the number used within the VSX manifest info.
1. The entire soution builds with the Release configuration (__Clean Solution__, __Rebuil Solution__)
1. All unit tests run successfully (no failed or ignored test cases)
1. You can uploade the new VSIX to the Visual Studio Marketplace.