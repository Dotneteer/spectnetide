---
layout: documents
categories:
  - "Tutorials v2"
title: "Setup ZX BASIC"
alias: export-a-z80-program
seqno: 1051
selector: tutorial
permalink: "getting-started/setup-zx-basic"
---

ZX BASIC (Boriel's BASIC) is a separate product maintained by Jose Rodriguez-Rosa (Boriel). You can find more information about this product in [Github](https://github.com/boriel/zxbasic). This site refers to several links that raise a 404 Error.

You can find the ZX BASIC Wiki [here](https://zxbasic.readthedocs.io/).

SpectNetIDE uses ZX BASIC as an external tool, so first, you have to install ZX BASIC to your computer. You can find the installation files here: [https://www.boriel.com/files/zxb/](https://www.boriel.com/files/zxb/).

> **_Note_**: The newest versions can be found on the bottom of the page.

To set up ZX BASIC integration with SpectNetIDE, follow these steps:

1. Download the ZXB version (with `zxbasic-n.n.n-win32.zip`) you want to install (I suggest you to dowload the latest stable release).
2. Unzip this file, and copy to contents into your preferred installation folder.

> **_Note_**: If you prefer the globally installed ZX BASIC version, please follow [these instructions](https://zxbasic.readthedocs.io/en/latest/installation/).

{:start="3"}

3. Start a **_command prompt_**, select the installation folder with the `CD` command, and then check the installation with this command line:

```
zxb --version
```

> **_Note_**: This command cannot be executed in Powershell on Windows (only with the CMD command-line prompt).

If the current version number is displayed, your ZX BASIC installation is successful.

{:start="4"}

4. Start Visual Studio 2019, and run the **Tools &rarr; Options** command.

![ZXB path]({{ site.baseurl }}/assets/images/tutorials/zxb-path-option.png)

{:start="5"}

5. In The **Spect.Net IDE** tab, set the **ZXB utility path** option to the folder where you installed ZX BASIC.

Alternatively, you can set up the **PATH** environment variable of your computer to point out to the ZX BASIC installation folder:

- In **File Explorer** right-click on **This PC** and run the **Properties** command.
- Select **Advanced System Settings**, and then click **Environment Variables**:

![Environment variables]({{ site.baseurl }}/assets/images/tutorials/env-var-1.png)

- Acording to your preference, you can edit User variables or System variables to add a PATH fragment that points to the ZX BASIC installation folder.

> Great! With these steps, you're ready to use ZX BASIC. Now you can proceed to the next tutorial, Use ZX BASIC.
