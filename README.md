# Description
New variant of the Network Toolkit (Telnet UI client), rewritten from Java to C#

You may build this toolkit from .sln file

# Functionality
- viewing the basic configuration of Eltex, QTech, Huawei, D-Link hardware
- the ability to register a config and a terminal on the hardware
- the general process of configuration is brought to the form 'all as one'
- and much more (WIP)

# Changelog
v 0.9.5
- added support for ethernet switches
- the ability to add specific VLANs by selecting the ones you need from the list of available

v 0.9.6
- regardless of the switch, the mechanism for viewing the configs is now the same for everyone
- added several commands for MA5608T: view MAC table, view service ports, control root modes from code
- added searching ONT by mac-address (MA5608T)
- added connection history log for MA5608T

v 0.9.7
- the ability to add a description to the MES3124 port
- removed the switch type check from the logger window and redone it using a checkbox
