

namespace ToolkitUI.TelnetCore
{
    public class TelnetCommander
    {
        private Telnet telnet;

        public TelnetCommander(string ip) { telnet = new Telnet(ip, 23); }

        public Telnet GetTelnet() { return telnet; }

//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Commands for xPON hardware
        // MA4000 and LTP-8X
        public void ShowConfigEltex(string dataUser) { SendCommand("show running-config interface ont " + dataUser); } // +
        public void ShowStateEltex(string dataUser) { SendCommand("show interface ont " + dataUser + " state"); } // +
        public void ShowUnactivatedEltex(string dataUser) { SendCommand("show interface ont " + dataUser + " unactivated"); } // +
        public void ShowOnlineEltex(string dataUser) { SendCommand("show interface ont " + dataUser + " online"); } // +
        public void ShowOfflineEltex(string dataUser) { SendCommand("show interface ont " + dataUser + " offline"); } // +
        public void ShowConnectionsEltex(string dataUser) { SendCommand("show interface ont " + dataUser + " connections"); } // +
        public void ShowMacTableEltex(string port_VID) { SendCommand("show mac interface gpon-port " + port_VID); }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Huawei MA56xx
        // xPON
        public void EnableAdmin_V1() { SendCommand("enable"); } // +
        public void EnableAdmin_V2() { SendCommand("config"); } // +
        public void EnableAdmin_V3(string frame_slot) { SendCommand("interface gpon " + frame_slot); } // +
        public void EnableAdmin_V4(string frame_slot) { SendCommand("interface mcu " + frame_slot); } // +
        public void DisableAdmin() { SendCommand("disable"); } // +
        public void CloseInterfaceMode() { SendCommand("return"); } // +

        // AFTER EnableAdmin_V1!!!
        public void ShowServicePortsHuawei(string dataUser, string ontID) { SendCommand("display service-port port " + dataUser + " ont " + ontID); } // +
        public void ShowMacTableHuawei(string dataUser, string ontID) { SendCommand("display mac-address port " + dataUser + " ont " + ontID); } // +
        public void ShowInfoONTBySlot(string slot) { SendCommand("display ont info " + slot + " all"); } // +
        public void SearchONTByMacHuawei(string mac_address) { SendCommand("display location " + mac_address); } // +
        public void ShowConfigHuawei(string dataUser) { SendCommand("display current-configuration ont " + dataUser); } // +
        public void ShowStateHuawei(string dataUser) { SendCommand("display ont info option run-state " + dataUser); } // +
        public void AutofindAllONT() { SendCommand("display ont autofind all"); } // +

        // AFTER EnableAdmin_V2!!!
        // AFTER EnableAdmin_V3!!!
        public void ShowPhysXErrorHuawei(string port) { SendCommand("display port state " + port); } // +
        public void ShowPortStateHuawei(string port_ID) { SendCommand("display ont port state " + port_ID + " eth-port all"); } // +
        public void AutofindONTBySlot(string slot) { SendCommand("display ont autofind " + slot); } // +
        public void ShowConnectionHistoryHuawei(string port_id) { SendCommand("display ont register-info " + port_id); } // +

        // AFTER EnableAdmin_V4!!!
        public void ShowOpticalInfoHuawei(string port) { SendCommand("display port ddm-info " + port); } // +
        public void ShowPortStatisticHuawei(string port) { SendCommand("display port statistics " + port); } // +

        // xDSL
        // AFTER Enabledmin_V1!!!
        public void ShowProfilesDSL() { SendCommand("display adsl line-profile"); }
        public void ShowLineParameters(string board) { SendCommand("display line operation board " + board); }

//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Commands for Ethernet hardware
        // MES-1124, MES-1024
        public void ShowMacTableMES(string dataUser) { SendCommand("show mac address-table interface FastEthernet1/0/" + dataUser); }
        public void DoCableTestMES(string dataUser) { SendCommand("test cable-diagnostics tdr interface FastEthernet1/0/" + dataUser); }
        public void ShowConfigMES(string dataUser) { SendCommand("show running-config interfaces FastEthernet1/0/" + dataUser); }
        public void ShowInterfacesDescriptionMES() { SendCommand("show interfaces description"); }
        public void ShowPhysXErrorsMES(string dataUser) { SendCommand("show interfaces FastEthernet1/0/" + dataUser); }

        // Aggegation MES3124
        public void ShowMacTableAggregation(string dataUser) { SendCommand("show mac address-table interface GigabitEthernet1/0/" + dataUser); }
        public void DoCableTestAggregation(string dataUser) { SendCommand("test cable-diagnostics tdr interface GigabitEthernet1/0/" + dataUser); }
        public void ShowConfigAggregation(string dataUser) { SendCommand("show running-config interfaces GigabitEthernet1/0/" + dataUser); }
        public void ShowInterfacesDescriptionAggregation() { SendCommand("show interfaces description"); }
        public void ShowPhysXErrorsAggregation(string dataUser) { SendCommand("show interfaces GigabitEthernet1/0/" + dataUser); }

        // QTech
        public void DoCableTestQSW(string dataUser) { SendCommand("virtual-cable-test interface ethernet1/0/" + dataUser); }
        public void ShowPhysXErrorQSW(string dataUser) { SendCommand("show interface ethernet1/0/" + dataUser); }
        public void LinkStatusQSW(string dataUser) { SendCommand("show interface ethernet status"); }
        public void ShowConfigQSW(string dataUser) { SendCommand("show running-config interface ethernet1/0/" + dataUser); }
        public void ShowMacTableQSW(string dataUser) { SendCommand("show mac-address-table interface ethernet1/0/" + dataUser); }
        public void EnableAdminQSW() { SendCommand("config"); }

        // D-Link 
        public void ShowConfigDlink(string dataUser) { SendCommand("show ports " + dataUser); }
        public void ShowPhysXErrorDlink(string dataUser) { SendCommand("show error ports " + dataUser); }
        public void ShowVlanPortsDlink(string dataUser) { SendCommand("show vlan ports " + dataUser); }
        public void ShowMacTableDlink(string dataUser) { SendCommand("show fdb port " + dataUser); }
        public void DoCableTestDLink(string dataUser) { SendCommand("cable diagnostic port " + dataUser); }
        public void EnableAdminDLink() { SendCommand("enable admin"); }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Other
        public void ShowAllVlanList() { SendCommand("show vlan"); }
        public void CloseConnection() { SendCommand("exit"); }
        public void CloseConnectionHuawei() { SendCommand("quit"); }
        public void Continue() { SendCommand("\n"); }

//---------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void SendCommand(string command) { telnet.SendCommand(command); }
    }
}
