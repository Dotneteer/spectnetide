---
layout: documents
categories: 
  - "Tutorials v2"
title:  "Setup ZX BASIC"
alias: export-a-z80-program
seqno: 1051
selector: tutorial
permalink: "getting-started/setup-zx-basic"
---

ZX BASIC (Boriel's BASIC) is a separate product maintained by Jose Rodriguez-Rosa (Boriel). You can find more information about this product in [Github](https://github.com/boriel/zxbasic). This site refers to several links that raise a 404 Error.

SpectNetIDE uses ZX BASIC as an external tool, so first, you have to install ZX BASIC to your computer. You can find the installation files here: [https://www.boriel.com/files/zxb/](https://www.boriel.com/files/zxb/). 

> Note: The newest versions can be found on the bottom of the page.

To set up ZX BASIC integration with SpectNetIDE, follow these steps:

1. Download the ZXB version (Win32) you want to install (I suggest you to dowload the latest stable release).
2. Unzip this file, and copy to contents into your preferred installation folder.
3. Start a command prompt, select the installation folder with the `CD` command, and then check the installation with this command line:

```
zxb --version
```

If the current version number is displayed, your ZX BASIC installation is successful.

{:start="4"}
4. Start Visual Studio 2019, and run the __Tools &rarr; Options__ command.

![ZXB path]({{ site.baseurl }}/assets/images/tutorials/zxb-path-option.png)

{:start="5"}
5. In The __Spect.Net IDE__ tab, set the __ZXB utility path__ option to the folder where you installed ZX BASIC. 

> __*Note*__: Alternatively, you can set up the __PATH__ environment variable of your computer to point out to the ZX BASIC installation folder.

With these steps, you're ready to use ZX BASIC. The next tutorial shows how you can write and run a ZX BASIC program in SpectNetIDE.