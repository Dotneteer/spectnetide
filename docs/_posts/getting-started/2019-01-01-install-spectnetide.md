---
layout: documents
categories: 
  - "Tutorials"
title:  "Install SpectNetIDE"
alias: install-spectnetide
seqno: 10
selector: tutorial
permalink: "getting-started/install-spectnetide"
---

__SpectNetIDE__ is an open source project with MIT license, and it is free to install. I implemented the ZX Spectrum IDE as a Visual Studio 2017/2019 extension (VSIX). Thus, you can run it only on Windows. Follow these steps to install the IDE:

1. You need Visual Studio installed on your machine. You do not have to pay to get a legal license for VS, SpectNetIDE works with the free Community edition seamlessly. You can download the Visual Studio edition of your choice from its [home page](https://visualstudio.microsoft.com/downloads/).

2. Select the Tools\|Extensions and Updates menu command. It displays a dialog to install VS extensions. Click the Online tab and type "SpectNetIDE" in the search box.
  
3. The dialog looks up the Visual Studio Marketplace and displays the SpectNetIDE extension. Click Download to start the installation.

![Extensions and Updates]({{ site.baseurl }}/assets/images/tutorials/extensions-dialog-in-vs.png)

{:start="4"}
4. The IDE prepares the downloaded package for setup. However, it will begin installing it only after you close Visual Studio.

![Ready to Install]({{ site.baseurl }}/assets/images/tutorials/extension-ready-to-install.png)

{:start="5"}
5. When you have closed all running instances of Visual Studio, the VSIX installer automatically starts the setup. First, you have to confirm the license terms and click Modify.

![Confirm VSIX Install]({{ site.baseurl }}/assets/images/tutorials/confirm-vsix-install.png)

{:start="6"}
6. The VSIX installer sets up the extension&mdash;it takes less than a minute&mdash;and signs when it completes.

![VSIX Install Completes]({{ site.baseurl }}/assets/images/tutorials/vsix-install-completes.png)

Now, SpectNetIDE is ready to use. When you start Visual Studio, in the Help\|About dialog you can check it's integrated with VS.

![SpectNetIde in About]({{ site.baseurl }}/assets/images/tutorials/spectnetide-in-about.png)

To get familiar with using the ZX Spectrum IDE, take a look at the other tutorials!

