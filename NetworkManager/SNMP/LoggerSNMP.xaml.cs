using NetworkManager.TelnetCore;
using SnmpSharpNet;
using System;
using System.IO;
using System.Windows;

namespace NetworkManager.SNMP
{
    public partial class LoggerSNMP : Window
    {
        public LoggerSNMP() { InitializeComponent(); }

        TextWriter writer;
        IpAddress ip;
        OctetString community;
        AgentParameters param;
        UdpTarget target;
        Pdu pdu;

        private void Start(object sender, RoutedEventArgs e)
        {
            community = new OctetString(communityField.Text);
            ip = new IpAddress(ipField.Text);

            param = new AgentParameters(community);
            param.Version = SnmpVersion.Ver2;

            target = new UdpTarget((System.Net.IPAddress)ip, 161, 2000, 3);

            pdu = new Pdu();
            pdu.VbList.Add(sysDescr.Text);
            pdu.VbList.Add(oidDescr.Text);

            // Get result
            SnmpV2Packet pack = (SnmpV2Packet)target.Request(pdu, param);

            writer = new RedirectOutput(logger);

            if (pack != null)
            {
                if (pack.Pdu.ErrorStatus != 0)
                    Console.WriteLine("Error in SNMP reply. Error {0} index {1}" + pack.Pdu.ErrorStatus, pack.Pdu.ErrorIndex);
                else
                {
                    Console.WriteLine("({0}) ({1}): {2}", pack.Pdu.VbList[0].Oid.ToString(),
                        SnmpConstants.GetTypeName(pack.Pdu.VbList[0].Value.Type), pack.Pdu.VbList[0].Value.ToString());
                    Console.WriteLine("({0}) ({1}): {2}", pack.Pdu.VbList[1].Oid.ToString(),
                        SnmpConstants.GetTypeName(pack.Pdu.VbList[1].Value.Type), pack.Pdu.VbList[1].Value.ToString());
                    Console.WriteLine("\n");
                }
            }
            else 
                Console.WriteLine("No response received from SNMP agent"); target.Close();

            Console.SetOut(writer);
        }

        private void WalkSNMP(object sender, RoutedEventArgs e)
        {
            community = new OctetString(communityField.Text);
            ip = new IpAddress(ipField.Text);

            param = new AgentParameters(community);
            param.Version = SnmpVersion.Ver2;

            target = new UdpTarget((System.Net.IPAddress)ip, 161, 2000, 3);

            Oid oid = new Oid(oidField.Text);
            Oid lastOid = (Oid)oid.Clone();

            pdu = new Pdu(PduType.GetBulk);
            pdu.NonRepeaters = 0;
            pdu.MaxRepetitions = 5;

            writer = new RedirectOutput(logger);

            while (lastOid != null)
            {
                if (pdu.RequestId != 0)
                    pdu.RequestId += 1;

                pdu.VbList.Clear();
                pdu.VbList.Add(lastOid);

                SnmpV2Packet pack = (SnmpV2Packet)target.Request(pdu, param);

                if (pack != null)
                {
                    if (pack.Pdu.ErrorStatus != 0)
                    {
                        Console.WriteLine("Error in SNMP reply. Error {0} index {1}", pack.Pdu.ErrorStatus, pack.Pdu.ErrorIndex);
                        lastOid = null;
                        break;
                    }
                    else
                    {
                        foreach (Vb v in pack.Pdu.VbList)
                        {
                            if (oid.IsRootOf(v.Oid))
                            {
                                Console.WriteLine("{0} ({1}): {2}", v.Oid.ToString(), SnmpConstants.GetTypeName(v.Value.Type), v.Value.ToString());

                                if (v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW)
                                    lastOid = null;
                                else
                                    lastOid = v.Oid;
                            }
                            else
                                lastOid = null;
                        }
                    }
                }
                else
                    Console.WriteLine("No response received from SNMP agent."); target.Close();
            }

            Console.SetOut(writer);
        }
    }
}
