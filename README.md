# Build client
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

v 0.9.8
- UI dark theme
- added xDSL support (WIP)
- added SNMP support

v 0.9.9
- removed logger window
- added connection status label
- other UI fixes

v 0.9.10
- added en/ru localizations
- remove old and trash code
- added configuring port mode for MES switches

v 1.0.0.0 FINAL in this year!!!
- there are few significant changes, but you should take into account that there may be problems with the connection if there is no response from the server for a long time. Also, the program does not fully implement DSL and SNMP technologies. Next year, I will definitely fix everything.


# Used libs
https://github.com/SuperTao/Telnet

https://github.com/rqx110/SnmpSharpNet
