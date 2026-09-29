using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using iTextSharp.text;
using iTextSharp.text.pdf;


namespace PortScanner
{
    public partial class PortScanner : Page
    {
        private CancellationTokenSource scanCancellation;


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }


        // =========================================================
        // INITIALIZE
        // =========================================================

        private void InitializePage()
        {
            btnCancel.Enabled = false;

            lblScanned.Text = "0";
            lblOpen.Text = "0";
            lblClosed.Text = "0";
            lblTimeout.Text = "0";
            lblScanTime.Text = "0 ms";

            lblTarget.Text = "-";
            lblResolvedIP.Text = "-";
            lblPortList.Text = "-";

            lblProgress.Text = "0%";

            progressBar.Style["width"] =
                "0%";

            gvResults.DataSource = null;
            gvResults.DataBind();
        }


        // =========================================================
        // START SCAN
        // =========================================================

        protected async void btnScan_Click(
            object sender,
            EventArgs e)
        {
            lblMessage.Visible = false;


            string target =
                txtTarget.Text.Trim();


            string portText =
                txtPorts.Text.Trim();


            if (string.IsNullOrWhiteSpace(target))
            {
                ShowMessage(
                    "Please enter a target host or IP address."
                );

                return;
            }


            if (string.IsNullOrWhiteSpace(portText))
            {
                ShowMessage(
                    "Please enter at least one port."
                );

                return;
            }


            List<int> ports;


            try
            {
                ports =
                    ParsePorts(portText);
            }
            catch (Exception ex)
            {
                ShowMessage(
                    ex.Message
                );

                return;
            }


            if (ports.Count == 0)
            {
                ShowMessage(
                    "No valid ports were entered."
                );

                return;
            }


            if (ports.Count > 100)
            {
                ShowMessage(
                    "Maximum 100 individual ports are allowed per scan."
                );

                return;
            }


            btnScan.Enabled = false;

            btnCancel.Enabled = true;

            btnScan.Text =
                "SCANNING...";


            scanCancellation =
                new CancellationTokenSource();


            Stopwatch stopwatch =
                Stopwatch.StartNew();


            try
            {
                ResetStatistics();


                lblTarget.Text =
                    Server.HtmlEncode(target);


                lblPortList.Text =
                    Server.HtmlEncode(
                        string.Join(
                            ", ",
                            ports
                        )
                    );


                // =================================================
                // RESOLVE TARGET
                // =================================================

                string ipAddress =
                    await ResolveHost(target);


                if (string.IsNullOrEmpty(ipAddress))
                {
                    ShowMessage(
                        "Unable to resolve the target host."
                    );

                    return;
                }


                lblResolvedIP.Text =
                    Server.HtmlEncode(
                        ipAddress
                    );


                // =================================================
                // RESULT TABLE
                // =================================================

                DataTable results =
                    CreateResultTable();


                int total =
                    ports.Count;


                int scanned = 0;

                int open = 0;

                int closed = 0;

                int timeout = 0;


                SemaphoreSlim semaphore =
                    new SemaphoreSlim(20);


                List<Task<PortResult>> tasks =
                    new List<Task<PortResult>>();


                foreach (int port in ports)
                {
                    tasks.Add(
                        ScanPortWithSemaphore(
                            ipAddress,
                            port,
                            semaphore,
                            scanCancellation.Token
                        )
                    );
                }


                PortResult[] scanResults =
                    await Task.WhenAll(
                        tasks
                    );


                // =================================================
                // PROCESS RESULTS
                // =================================================

                foreach (
                    PortResult result
                    in scanResults.OrderBy(
                        x => x.Port
                    )
                )
                {
                    if (
                        scanCancellation
                            .IsCancellationRequested
                    )
                    {
                        break;
                    }


                    scanned++;


                    if (
                        result.Status ==
                        "OPEN"
                    )
                    {
                        open++;
                    }
                    else if (
                        result.Status ==
                        "TIMEOUT"
                    )
                    {
                        timeout++;
                    }
                    else
                    {
                        closed++;
                    }


                    DataRow row =
                        results.NewRow();


                    row["Port"] =
                        result.Port;


                    row["Status"] =
                        result.Status;


                    row["Service"] =
                        GetServiceName(
                            result.Port
                        );


                    row["Version"] =
                        result.Version;


                    row["Risk"] =
                        GetRisk(
                            result.Port
                        );


                    row["Response"] =
                        result.ResponseTime +
                        " ms";


                    results.Rows.Add(
                        row
                    );


                    UpdateProgress(
                        scanned,
                        total
                    );
                }


                // =================================================
                // DISPLAY RESULTS
                // =================================================

                gvResults.DataSource =
                    results;

                gvResults.DataBind();


                /*
                 * Store the actual DataTable.
                 *
                 * PDF uses this table directly.
                 */

                Session[
                    "NetScanResults"
                ] = results;


                Session[
                    "NetScanTarget"
                ] = target;


                Session[
                    "NetScanIP"
                ] = ipAddress;


                // =================================================
                // STATISTICS
                // =================================================

                lblScanned.Text =
                    scanned.ToString();


                lblOpen.Text =
                    open.ToString();


                lblClosed.Text =
                    closed.ToString();


                lblTimeout.Text =
                    timeout.ToString();


                stopwatch.Stop();


                lblScanTime.Text =
                    stopwatch.ElapsedMilliseconds +
                    " ms";


                if (
                    scanCancellation
                        .IsCancellationRequested
                )
                {
                    ShowMessage(
                        "Scan cancelled."
                    );
                }
                else
                {
                    ShowMessage(
                        "Scan completed successfully. " +
                        scanned +
                        " specific ports were analyzed."
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage(
                    "Scanner error: " +
                    ex.Message
                );
            }
            finally
            {
                stopwatch.Stop();


                btnScan.Enabled = true;

                btnCancel.Enabled = false;

                btnScan.Text =
                    "START SCAN";


                if (
                    scanCancellation != null
                )
                {
                    scanCancellation.Dispose();

                    scanCancellation = null;
                }
            }
        }


        // =========================================================
        // CANCEL
        // =========================================================

        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            if (
                scanCancellation != null
            )
            {
                scanCancellation.Cancel();

                ShowMessage(
                    "Cancellation requested..."
                );
            }
        }


        // =========================================================
        // PARSE PORTS
        // =========================================================

        private List<int> ParsePorts(
            string input)
        {
            List<int> ports =
                new List<int>();


            string[] parts =
                input.Split(
                    new char[]
                    {
                        ',',
                        ';',
                        ' ',
                        '\t'
                    },
                    StringSplitOptions
                        .RemoveEmptyEntries
                );


            foreach (
                string part
                in parts
            )
            {
                int port;


                if (
                    !int.TryParse(
                        part.Trim(),
                        out port
                    )
                )
                {
                    throw new Exception(
                        "Invalid port: " +
                        part
                    );
                }


                if (
                    port < 1 ||
                    port > 65535
                )
                {
                    throw new Exception(
                        "Port " +
                        port +
                        " must be between 1 and 65535."
                    );
                }


                if (
                    !ports.Contains(port)
                )
                {
                    ports.Add(port);
                }
            }


            ports.Sort();


            return ports;
        }


        // =========================================================
        // SEMAPHORE
        // =========================================================

        private async Task<PortResult>
            ScanPortWithSemaphore(
                string ip,
                int port,
                SemaphoreSlim semaphore,
                CancellationToken token)
        {
            await semaphore.WaitAsync(
                token
            );


            try
            {
                return await ScanPort(
                    ip,
                    port,
                    token
                );
            }
            finally
            {
                semaphore.Release();
            }
        }


        // =========================================================
        // TCP PORT SCAN
        // =========================================================

        private async Task<PortResult>
            ScanPort(
                string ip,
                int port,
                CancellationToken token)
        {
            Stopwatch stopwatch =
                Stopwatch.StartNew();


            using (
                TcpClient client =
                new TcpClient()
            )
            {
                try
                {
                    Task connectTask =
                        client.ConnectAsync(
                            ip,
                            port
                        );


                    Task timeoutTask =
                        Task.Delay(
                            1200,
                            token
                        );


                    Task completed =
                        await Task.WhenAny(
                            connectTask,
                            timeoutTask
                        );


                    stopwatch.Stop();


                    if (
                        completed ==
                        connectTask
                    )
                    {
                        await connectTask;


                        string version =
                            await DetectVersion(
                                ip,
                                port,
                                token
                            );


                        return new PortResult
                        {
                            Port = port,

                            Status = "OPEN",

                            Version = version,

                            ResponseTime =
                                stopwatch
                                    .ElapsedMilliseconds
                        };
                    }


                    return new PortResult
                    {
                        Port = port,

                        Status = "TIMEOUT",

                        Version =
                            "No response",

                        ResponseTime =
                            stopwatch
                                .ElapsedMilliseconds
                    };
                }
                catch (
                    OperationCanceledException
                )
                {
                    return new PortResult
                    {
                        Port = port,

                        Status = "TIMEOUT",

                        Version =
                            "Cancelled",

                        ResponseTime =
                            stopwatch
                                .ElapsedMilliseconds
                    };
                }
                catch
                {
                    stopwatch.Stop();


                    return new PortResult
                    {
                        Port = port,

                        Status = "CLOSED",

                        Version = "-",

                        ResponseTime =
                            stopwatch
                                .ElapsedMilliseconds
                    };
                }
            }
        }


        // =========================================================
        // VERSION DETECTION
        // =========================================================

        private async Task<string>
            DetectVersion(
                string ip,
                int port,
                CancellationToken token)
        {
            try
            {
                if (
                    port == 80 ||
                    port == 8000 ||
                    port == 8080 ||
                    port == 3000 ||
                    port == 5000
                )
                {
                    return await DetectHttpBanner(
                        ip,
                        port,
                        false,
                        token
                    );
                }


                if (
                    port == 443 ||
                    port == 8443
                )
                {
                    return await DetectHttpsBanner(
                        ip,
                        port,
                        token
                    );
                }


                if (
                    port == 21 ||
                    port == 22 ||
                    port == 25 ||
                    port == 110 ||
                    port == 143 ||
                    port == 587
                )
                {
                    return await DetectTcpBanner(
                        ip,
                        port,
                        token
                    );
                }


                return "No version banner exposed";
            }
            catch
            {
                return "Banner unavailable";
            }
        }


        // =========================================================
        // HTTP
        // =========================================================

        private async Task<string>
            DetectHttpBanner(
                string ip,
                int port,
                bool https,
                CancellationToken token)
        {
            try
            {
                using (
                    TcpClient client =
                    new TcpClient()
                )
                {
                    Task connectTask =
                        client.ConnectAsync(
                            ip,
                            port
                        );


                    Task timeoutTask =
                        Task.Delay(
                            2000,
                            token
                        );


                    Task completed =
                        await Task.WhenAny(
                            connectTask,
                            timeoutTask
                        );


                    if (
                        completed !=
                        connectTask
                    )
                    {
                        return
                            "HTTP banner timeout";
                    }


                    await connectTask;


                    Stream stream =
                        client.GetStream();


                    using (stream)
                    {
                        if (https)
                        {
                            SslStream sslStream =
                                new SslStream(
                                    stream,
                                    false,
                                    ValidateCertificate
                                );


                            try
                            {
                                await sslStream
                                    .AuthenticateAsClientAsync(
                                        ip
                                    );
                            }
                            catch
                            {
                                return
                                    "HTTPS service detected; TLS banner unavailable";
                            }


                            stream =
                                sslStream;
                        }


                        string request =
                            "HEAD / HTTP/1.0\r\n" +
                            "Host: " +
                            ip +
                            "\r\n" +
                            "User-Agent: NetScan-Pro\r\n" +
                            "Connection: close\r\n\r\n";


                        byte[] data =
                            Encoding.ASCII.GetBytes(
                                request
                            );


                        await stream.WriteAsync(
                            data,
                            0,
                            data.Length,
                            token
                        );


                        byte[] buffer =
                            new byte[8192];


                        Task<int> readTask =
                            stream.ReadAsync(
                                buffer,
                                0,
                                buffer.Length,
                                token
                            );


                        Task readTimeout =
                            Task.Delay(
                                2500,
                                token
                            );


                        Task readCompleted =
                            await Task.WhenAny(
                                readTask,
                                readTimeout
                            );


                        if (
                            readCompleted !=
                            readTask
                        )
                        {
                            return
                                "HTTP server detected";
                        }


                        int bytes =
                            await readTask;


                        if (bytes <= 0)
                        {
                            return
                                "HTTP server detected";
                        }


                        string response =
                            Encoding.ASCII.GetString(
                                buffer,
                                0,
                                bytes
                            );


                        string server =
                            ExtractHeader(
                                response,
                                "Server"
                            );


                        string poweredBy =
                            ExtractHeader(
                                response,
                                "X-Powered-By"
                            );


                        string status =
                            GetFirstLine(
                                response
                            );


                        if (
                            !string.IsNullOrWhiteSpace(
                                server
                            )
                        )
                        {
                            if (
                                !string.IsNullOrWhiteSpace(
                                    poweredBy
                                )
                            )
                            {
                                return
                                    "Server: " +
                                    server +
                                    " | X-Powered-By: " +
                                    poweredBy;
                            }


                            return
                                "Server: " +
                                server;
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                poweredBy
                            )
                        )
                        {
                            return
                                "X-Powered-By: " +
                                poweredBy;
                        }


                        if (
                            !string.IsNullOrWhiteSpace(
                                status
                            )
                        )
                        {
                            return status;
                        }


                        return
                            https
                                ? "HTTPS service detected"
                                : "HTTP service detected";
                    }
                }
            }
            catch
            {
                return
                    https
                        ? "HTTPS banner unavailable"
                        : "HTTP banner unavailable";
            }
        }


        // =========================================================
        // HTTPS
        // =========================================================

        private async Task<string>
            DetectHttpsBanner(
                string ip,
                int port,
                CancellationToken token)
        {
            return await DetectHttpBanner(
                ip,
                port,
                true,
                token
            );
        }


        // =========================================================
        // CERTIFICATE VALIDATION
        // =========================================================

        private bool ValidateCertificate(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            SslPolicyErrors errors)
        {
            return true;
        }


        // =========================================================
        // TCP BANNER
        // =========================================================

        private async Task<string>
            DetectTcpBanner(
                string ip,
                int port,
                CancellationToken token)
        {
            try
            {
                using (
                    TcpClient client =
                    new TcpClient()
                )
                {
                    Task connectTask =
                        client.ConnectAsync(
                            ip,
                            port
                        );


                    Task timeoutTask =
                        Task.Delay(
                            1800,
                            token
                        );


                    Task completed =
                        await Task.WhenAny(
                            connectTask,
                            timeoutTask
                        );


                    if (
                        completed !=
                        connectTask
                    )
                    {
                        return
                            "Banner timeout";
                    }


                    await connectTask;


                    using (
                        NetworkStream stream =
                        client.GetStream()
                    )
                    {
                        byte[] buffer =
                            new byte[4096];


                        Task<int> readTask =
                            stream.ReadAsync(
                                buffer,
                                0,
                                buffer.Length,
                                token
                            );


                        Task readTimeout =
                            Task.Delay(
                                1800,
                                token
                            );


                        Task readCompleted =
                            await Task.WhenAny(
                                readTask,
                                readTimeout
                            );


                        if (
                            readCompleted !=
                            readTask
                        )
                        {
                            return
                                "No banner exposed";
                        }


                        int bytes =
                            await readTask;


                        if (bytes <= 0)
                        {
                            return
                                "No banner exposed";
                        }


                        string banner =
                            Encoding.ASCII.GetString(
                                buffer,
                                0,
                                bytes
                            );


                        banner =
                            banner.Replace(
                                "\0",
                                ""
                            );


                        banner =
                            banner.Replace(
                                "\r",
                                " "
                            );


                        banner =
                            banner.Replace(
                                "\n",
                                " "
                            );


                        banner =
                            banner.Trim();


                        if (
                            banner.Length == 0
                        )
                        {
                            return
                                "No banner exposed";
                        }


                        if (
                            banner.Length > 500
                        )
                        {
                            banner =
                                banner.Substring(
                                    0,
                                    500
                                );
                        }


                        return banner;
                    }
                }
            }
            catch
            {
                return
                    "Banner unavailable";
            }
        }


        // =========================================================
        // HTTP HEADER
        // =========================================================

        private string ExtractHeader(
            string response,
            string headerName)
        {
            string[] lines =
                response.Split(
                    new[]
                    {
                        "\r\n"
                    },
                    StringSplitOptions.None
                );


            foreach (
                string line
                in lines
            )
            {
                if (
                    line.StartsWith(
                        headerName + ":",
                        StringComparison
                            .OrdinalIgnoreCase
                    )
                )
                {
                    int index =
                        line.IndexOf(
                            ':'
                        );


                    if (index >= 0)
                    {
                        return
                            line.Substring(
                                index + 1
                            ).Trim();
                    }
                }
            }


            return "";
        }


        // =========================================================
        // FIRST HTTP LINE
        // =========================================================

        private string GetFirstLine(
            string response)
        {
            if (
                string.IsNullOrWhiteSpace(
                    response
                )
            )
            {
                return "";
            }


            string[] lines =
                response.Split(
                    new[]
                    {
                        "\r\n"
                    },
                    StringSplitOptions.None
                );


            if (
                lines.Length > 0
            )
            {
                return lines[0].Trim();
            }


            return "";
        }


        // =========================================================
        // DNS
        // =========================================================

        private async Task<string>
            ResolveHost(
                string host)
        {
            try
            {
                IPAddress address;


                if (
                    IPAddress.TryParse(
                        host,
                        out address
                    )
                )
                {
                    return address.ToString();
                }


                IPHostEntry entry =
                    await Dns.GetHostEntryAsync(
                        host
                    );


                foreach (
                    IPAddress ip
                    in entry.AddressList
                )
                {
                    if (
                        ip.AddressFamily ==
                        AddressFamily.InterNetwork
                    )
                    {
                        return ip.ToString();
                    }
                }


                if (
                    entry.AddressList.Length > 0
                )
                {
                    return
                        entry.AddressList[0]
                            .ToString();
                }
            }
            catch
            {
                return null;
            }


            return null;
        }


        // =========================================================
        // SERVICE NAMES
        // =========================================================

        private string GetServiceName(
            int port)
        {
            switch (port)
            {
                case 20:
                    return "FTP Data";

                case 21:
                    return "FTP";

                case 22:
                    return "SSH";

                case 23:
                    return "Telnet";

                case 25:
                    return "SMTP";

                case 53:
                    return "DNS";

                case 67:
                    return "DHCP";

                case 68:
                    return "DHCP";

                case 69:
                    return "TFTP";

                case 80:
                    return "HTTP";

                case 110:
                    return "POP3";

                case 111:
                    return "RPC";

                case 123:
                    return "NTP";

                case 135:
                    return "MS RPC";

                case 139:
                    return "NetBIOS";

                case 143:
                    return "IMAP";

                case 161:
                    return "SNMP";

                case 389:
                    return "LDAP";

                case 443:
                    return "HTTPS";

                case 445:
                    return "SMB";

                case 465:
                    return "SMTPS";

                case 587:
                    return "SMTP";

                case 636:
                    return "LDAPS";

                case 993:
                    return "IMAPS";

                case 995:
                    return "POP3S";

                case 1080:
                    return "SOCKS";

                case 1433:
                    return "MS SQL";

                case 1521:
                    return "Oracle";

                case 3306:
                    return "MySQL";

                case 3389:
                    return "RDP";

                case 5432:
                    return "PostgreSQL";

                case 5900:
                    return "VNC";

                case 6379:
                    return "Redis";

                case 6443:
                    return "Kubernetes";

                case 8000:
                    return "HTTP Alt";

                case 8080:
                    return "HTTP Proxy";

                case 8443:
                    return "HTTPS Alt";

                case 9200:
                    return "Elasticsearch";

                case 27017:
                    return "MongoDB";

                default:
                    return "Unknown";
            }
        }


        // =========================================================
        // RISK
        // =========================================================

        private string GetRisk(
            int port)
        {
            switch (port)
            {
                case 21:
                case 23:
                case 445:
                case 1433:
                case 1521:
                case 3306:
                case 3389:
                case 5432:
                case 5900:
                case 6379:
                case 9200:
                case 27017:

                    return "HIGH";


                case 22:
                case 25:
                case 110:
                case 139:
                case 161:
                case 389:
                case 8080:
                case 8443:

                    return "MEDIUM";


                case 80:
                case 443:

                    return "LOW";


                default:

                    return "INFO";
            }
        }


        // =========================================================
        // RESULT TABLE
        // =========================================================

        private DataTable CreateResultTable()
        {
            DataTable table =
                new DataTable();


            table.Columns.Add(
                "Port",
                typeof(int)
            );


            table.Columns.Add(
                "Status",
                typeof(string)
            );


            table.Columns.Add(
                "Service",
                typeof(string)
            );


            table.Columns.Add(
                "Version",
                typeof(string)
            );


            table.Columns.Add(
                "Risk",
                typeof(string)
            );


            table.Columns.Add(
                "Response",
                typeof(string)
            );


            return table;
        }


        // =========================================================
        // PROGRESS
        // =========================================================

        private void UpdateProgress(
            int scanned,
            int total)
        {
            if (total <= 0)
            {
                return;
            }


            int percentage =
                (int)(
                    scanned /
                    (double)total *
                    100
                );


            if (percentage > 100)
            {
                percentage = 100;
            }


            lblProgress.Text =
                percentage + "%";


            progressBar.Style["width"] =
                percentage + "%";
        }


        // =========================================================
        // RESET
        // =========================================================

        private void ResetStatistics()
        {
            lblScanned.Text = "0";

            lblOpen.Text = "0";

            lblClosed.Text = "0";

            lblTimeout.Text = "0";

            lblScanTime.Text = "0 ms";

            lblProgress.Text = "0%";

            progressBar.Style["width"] =
                "0%";
        }


        // =========================================================
        // MESSAGE
        // =========================================================

        private void ShowMessage(
            string message)
        {
            lblMessage.Text =
                Server.HtmlEncode(
                    message
                );

            lblMessage.Visible =
                true;
        }


        // =========================================================
        // STATUS CSS
        // =========================================================

        public string GetStatusClass(
            string status)
        {
            if (
                status.Equals(
                    "OPEN",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "open-badge";
            }


            if (
                status.Equals(
                    "TIMEOUT",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "timeout-badge";
            }


            return "closed-badge";
        }


        // =========================================================
        // RISK CSS
        // =========================================================

        public string GetRiskClass(
            string risk)
        {
            if (
                risk.Equals(
                    "HIGH",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "risk-high";
            }


            if (
                risk.Equals(
                    "MEDIUM",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "risk-medium";
            }


            if (
                risk.Equals(
                    "LOW",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "risk-low";
            }


            return "risk-info";
        }


        // =========================================================
        // CLEAR RESULTS
        // =========================================================

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            Session.Remove(
                "NetScanResults"
            );

            Session.Remove(
                "NetScanTarget"
            );

            Session.Remove(
                "NetScanIP"
            );


            gvResults.DataSource =
                null;

            gvResults.DataBind();


            txtTarget.Text = "";

            txtPorts.Text = "";


            InitializePage();


            ShowMessage(
                "Results cleared."
            );
        }


        // =========================================================
        // PDF REPORT
        // =========================================================

        protected void btnPDF_Click(
            object sender,
            EventArgs e)
        {
            DataTable results =
                Session[
                    "NetScanResults"
                ] as DataTable;


            if (
                results == null ||
                results.Rows.Count == 0
            )
            {
                ShowMessage(
                    "Run a scan before generating the PDF report."
                );

                return;
            }


            string target =
                Convert.ToString(
                    Session[
                        "NetScanTarget"
                    ]
                );


            string ip =
                Convert.ToString(
                    Session[
                        "NetScanIP"
                    ]
                );


            using (
                MemoryStream memoryStream =
                new MemoryStream()
            )
            {
                // =================================================
                // LANDSCAPE A4
                // =================================================

                Document document =
                    new Document(
                        PageSize.A4.Rotate(),
                        25,
                        25,
                        35,
                        35
                    );


                PdfWriter writer =
                    PdfWriter.GetInstance(
                        document,
                        memoryStream
                    );


                writer.CloseStream = false;


                document.Open();


                // =================================================
                // FONTS
                // =================================================

                Font titleFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        19,
                        BaseColor.WHITE
                    );


                Font subtitleFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        8,
                        BaseColor.WHITE
                    );


                Font sectionFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        9,
                        BaseColor.WHITE
                    );


                Font labelFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        8,
                        new BaseColor(
                            40,
                            40,
                            40
                        )
                    );


                Font valueFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        8,
                        new BaseColor(
                            40,
                            40,
                            40
                        )
                    );


                Font tableHeaderFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA_BOLD,
                        7,
                        BaseColor.WHITE
                    );


                Font tableFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        7,
                        new BaseColor(
                            35,
                            35,
                            35
                        )
                    );


                Font smallFont =
                    FontFactory.GetFont(
                        FontFactory.HELVETICA,
                        7,
                        new BaseColor(
                            80,
                            80,
                            80
                        )
                    );


                // =================================================
                // REPORT HEADER
                // =================================================

                PdfPTable headerTable =
                    new PdfPTable(1);


                headerTable.WidthPercentage =
                    100;


                PdfPCell headerCell =
                    new PdfPCell();


                headerCell.BackgroundColor =
                    new BaseColor(
                        37,
                        65,
                        170
                    );


                headerCell.Border =
                    Rectangle.NO_BORDER;


                headerCell.PaddingTop = 12;

                headerCell.PaddingBottom = 12;

                headerCell.PaddingLeft = 14;

                headerCell.PaddingRight = 14;


                headerCell.AddElement(
                    new Paragraph(
                        "NETSCAN PRO",
                        titleFont
                    )
                );


                headerCell.AddElement(
                    new Paragraph(
                        "Advanced TCP Port Scanner Report",
                        subtitleFont
                    )
                );


                headerTable.AddCell(
                    headerCell
                );


                document.Add(
                    headerTable
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // SCAN INFORMATION
                // =================================================

                AddPdfSectionTitle(
                    document,
                    "SCAN INFORMATION",
                    sectionFont
                );


                PdfPTable information =
                    new PdfPTable(2);


                information.WidthPercentage =
                    100;


                information.SetWidths(
                    new float[]
                    {
                        1.2f,
                        4.8f
                    }
                );


                AddPdfInfoRow(
                    information,
                    "Target",
                    target,
                    labelFont,
                    valueFont
                );


                AddPdfInfoRow(
                    information,
                    "Resolved IP",
                    ip,
                    labelFont,
                    valueFont
                );


                AddPdfInfoRow(
                    information,
                    "Protocol",
                    "TCP",
                    labelFont,
                    valueFont
                );


                AddPdfInfoRow(
                    information,
                    "Ports Scanned",
                    lblPortList.Text,
                    labelFont,
                    valueFont
                );


                AddPdfInfoRow(
                    information,
                    "Scan Duration",
                    lblScanTime.Text,
                    labelFont,
                    valueFont
                );


                AddPdfInfoRow(
                    information,
                    "Generated",
                    DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss"
                    ),
                    labelFont,
                    valueFont
                );


                document.Add(
                    information
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // SUMMARY
                // =================================================

                AddPdfSectionTitle(
                    document,
                    "SCAN SUMMARY",
                    sectionFont
                );


                PdfPTable summary =
                    new PdfPTable(4);


                summary.WidthPercentage =
                    100;


                AddPdfSummaryCell(
                    summary,
                    "SCANNED",
                    lblScanned.Text
                );


                AddPdfSummaryCell(
                    summary,
                    "OPEN",
                    lblOpen.Text
                );


                AddPdfSummaryCell(
                    summary,
                    "CLOSED",
                    lblClosed.Text
                );


                AddPdfSummaryCell(
                    summary,
                    "TIMEOUT",
                    lblTimeout.Text
                );


                document.Add(
                    summary
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // PORT RESULTS
                // =================================================

                AddPdfSectionTitle(
                    document,
                    "PORT SCAN RESULTS",
                    sectionFont
                );


                PdfPTable resultsTable =
                    new PdfPTable(6);


                resultsTable.WidthPercentage =
                    100;


                resultsTable.SetWidths(
                    new float[]
                    {
                        0.55f,
                        0.80f,
                        1.10f,
                        4.30f,
                        0.70f,
                        0.85f
                    }
                );


                resultsTable.HeaderRows =
                    1;


                resultsTable.SplitRows =
                    true;


                resultsTable.SplitLate =
                    false;


                string[] headers =
                {
                    "PORT",
                    "STATUS",
                    "SERVICE",
                    "CURRENT VERSION / BANNER",
                    "RISK",
                    "LATENCY"
                };


                foreach (
                    string header
                    in headers
                )
                {
                    PdfPCell cell =
                        new PdfPCell(
                            new Phrase(
                                header,
                                tableHeaderFont
                            )
                        );


                    cell.BackgroundColor =
                        new BaseColor(
                            15,
                            23,
                            42
                        );


                    cell.PaddingTop = 6;

                    cell.PaddingBottom = 6;

                    cell.PaddingLeft = 5;

                    cell.PaddingRight = 5;


                    cell.VerticalAlignment =
                        Element.ALIGN_MIDDLE;


                    resultsTable.AddCell(
                        cell
                    );
                }


                // =================================================
                // READ DIRECTLY FROM DATATABLE
                // =================================================

                foreach (
                    DataRow row
                    in results.Rows
                )
                {
                    AddPdfResultCell(
                        resultsTable,
                        Convert.ToString(
                            row["Port"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        resultsTable,
                        Convert.ToString(
                            row["Status"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        resultsTable,
                        Convert.ToString(
                            row["Service"]
                        ),
                        tableFont
                    );


                    AddPdfVersionCell(
                        resultsTable,
                        Convert.ToString(
                            row["Version"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        resultsTable,
                        Convert.ToString(
                            row["Risk"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        resultsTable,
                        Convert.ToString(
                            row["Response"]
                        ),
                        tableFont
                    );
                }


                document.Add(
                    resultsTable
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // OPEN PORT DETAILS
                // =================================================

                AddPdfSectionTitle(
                    document,
                    "OPEN PORT DETAILS",
                    sectionFont
                );


                PdfPTable openTable =
                    new PdfPTable(4);


                openTable.WidthPercentage =
                    100;


                openTable.SetWidths(
                    new float[]
                    {
                        0.8f,
                        1.4f,
                        5.0f,
                        0.8f
                    }
                );


                openTable.HeaderRows =
                    1;


                string[] openHeaders =
                {
                    "PORT",
                    "SERVICE",
                    "VERSION / BANNER",
                    "RISK"
                };


                foreach (
                    string header
                    in openHeaders
                )
                {
                    PdfPCell cell =
                        new PdfPCell(
                            new Phrase(
                                header,
                                tableHeaderFont
                            )
                        );


                    cell.BackgroundColor =
                        new BaseColor(
                            30,
                            41,
                            59
                        );


                    cell.Padding = 5;


                    openTable.AddCell(
                        cell
                    );
                }


                bool foundOpenPort =
                    false;


                foreach (
                    DataRow row
                    in results.Rows
                )
                {
                    string status =
                        Convert.ToString(
                            row["Status"]
                        );


                    if (
                        !status.Equals(
                            "OPEN",
                            StringComparison
                                .OrdinalIgnoreCase
                        )
                    )
                    {
                        continue;
                    }


                    foundOpenPort =
                        true;


                    AddPdfResultCell(
                        openTable,
                        Convert.ToString(
                            row["Port"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        openTable,
                        Convert.ToString(
                            row["Service"]
                        ),
                        tableFont
                    );


                    AddPdfVersionCell(
                        openTable,
                        Convert.ToString(
                            row["Version"]
                        ),
                        tableFont
                    );


                    AddPdfResultCell(
                        openTable,
                        Convert.ToString(
                            row["Risk"]
                        ),
                        tableFont
                    );
                }


                if (!foundOpenPort)
                {
                    PdfPCell noOpenCell =
                        new PdfPCell(
                            new Phrase(
                                "No open ports were detected in the selected port list.",
                                tableFont
                            )
                        );


                    noOpenCell.Colspan =
                        4;


                    noOpenCell.Padding =
                        7;


                    openTable.AddCell(
                        noOpenCell
                    );
                }


                document.Add(
                    openTable
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // VERSION INFORMATION
                // =================================================

                AddPdfSectionTitle(
                    document,
                    "VERSION / BANNER INFORMATION",
                    sectionFont
                );


                PdfPTable versionInfo =
                    new PdfPTable(1);


                versionInfo.WidthPercentage =
                    100;


                PdfPCell versionInfoCell =
                    new PdfPCell();


                versionInfoCell.BackgroundColor =
                    new BaseColor(
                        241,
                        245,
                        249
                    );


                versionInfoCell.Padding =
                    9;


                versionInfoCell.AddElement(
                    new Paragraph(
                        "The scanner reports service version or banner information only when the target service exposes it. A missing banner means that this scanner did not receive usable version information from the service.",
                        smallFont
                    )
                );


                versionInfo.AddCell(
                    versionInfoCell
                );


                document.Add(
                    versionInfo
                );


                document.Add(
                    new Paragraph(" ")
                );


                // =================================================
                // FOOTER
                // =================================================

                Paragraph footer =
                    new Paragraph(
                        "NetScan Pro | ASP.NET Web Forms | Authorized security testing only",
                        smallFont
                    );


                footer.Alignment =
                    Element.ALIGN_CENTER;


                document.Add(
                    footer
                );


                // =================================================
                // CLOSE DOCUMENT
                // =================================================

                document.Close();


                byte[] pdf =
                    memoryStream.ToArray();


                Response.Clear();

                Response.Buffer = true;


                Response.ContentType =
                    "application/pdf";


                Response.AddHeader(
                    "Content-Disposition",
                    "attachment; filename=NetScan_Report_" +
                    DateTime.Now.ToString(
                        "yyyyMMdd_HHmmss"
                    ) +
                    ".pdf"
                );


                Response.Cache.SetCacheability(
                    HttpCacheability.NoCache
                );


                Response.OutputStream.Write(
                    pdf,
                    0,
                    pdf.Length
                );


                Response.Flush();


                HttpContext.Current
                    .ApplicationInstance
                    .CompleteRequest();
            }
        }


        // =========================================================
        // PDF SECTION TITLE
        // =========================================================

        private void AddPdfSectionTitle(
            Document document,
            string title,
            Font font)
        {
            PdfPTable table =
                new PdfPTable(1);


            table.WidthPercentage =
                100;


            PdfPCell cell =
                new PdfPCell(
                    new Phrase(
                        title,
                        font
                    )
                );


            cell.BackgroundColor =
                new BaseColor(
                    37,
                    99,
                    235
                );


            cell.BorderColor =
                new BaseColor(
                    37,
                    99,
                    235
                );


            cell.PaddingTop = 6;

            cell.PaddingBottom = 6;

            cell.PaddingLeft = 7;


            table.AddCell(cell);


            document.Add(table);
        }


        // =========================================================
        // PDF INFORMATION ROW
        // =========================================================

        private void AddPdfInfoRow(
            PdfPTable table,
            string label,
            string value,
            Font labelFont,
            Font valueFont)
        {
            PdfPCell labelCell =
                new PdfPCell(
                    new Phrase(
                        CleanPdfText(label),
                        labelFont
                    )
                );


            PdfPCell valueCell =
                new PdfPCell(
                    new Phrase(
                        CleanPdfText(value),
                        valueFont
                    )
                );


            labelCell.BackgroundColor =
                new BaseColor(
                    248,
                    250,
                    252
                );


            labelCell.Padding = 6;

            valueCell.Padding = 6;


            labelCell.VerticalAlignment =
                Element.ALIGN_MIDDLE;


            valueCell.VerticalAlignment =
                Element.ALIGN_MIDDLE;


            valueCell.NoWrap =
                false;


            table.AddCell(
                labelCell
            );


            table.AddCell(
                valueCell
            );
        }


        // =========================================================
        // PDF SUMMARY CELL
        // =========================================================

        private void AddPdfSummaryCell(
            PdfPTable table,
            string label,
            string value)
        {
            Font labelFont =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    6.5f,
                    BaseColor.WHITE
                );


            Font numberFont =
                FontFactory.GetFont(
                    FontFactory.HELVETICA_BOLD,
                    14,
                    BaseColor.WHITE
                );


            PdfPCell cell =
                new PdfPCell();


            cell.BackgroundColor =
                new BaseColor(
                    30,
                    41,
                    59
                );


            cell.BorderColor =
                new BaseColor(
                    80,
                    90,
                    105
                );


            cell.Padding = 8;


            cell.AddElement(
                new Paragraph(
                    label,
                    labelFont
                )
            );


            cell.AddElement(
                new Paragraph(
                    value,
                    numberFont
                )
            );


            table.AddCell(
                cell
            );
        }


        // =========================================================
        // PDF RESULT CELL
        // =========================================================

        private void AddPdfResultCell(
            PdfPTable table,
            string value,
            Font font)
        {
            PdfPCell cell =
                new PdfPCell(
                    new Phrase(
                        CleanPdfText(value),
                        font
                    )
                );


            cell.PaddingTop = 5;

            cell.PaddingBottom = 5;

            cell.PaddingLeft = 5;

            cell.PaddingRight = 5;


            cell.VerticalAlignment =
                Element.ALIGN_TOP;


            cell.NoWrap =
                false;


            table.AddCell(
                cell
            );
        }


        // =========================================================
        // PDF VERSION CELL
        // =========================================================

        private void AddPdfVersionCell(
            PdfPTable table,
            string value,
            Font font)
        {
            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                value =
                    "No version/banner exposed";
            }


            PdfPCell cell =
                new PdfPCell(
                    new Phrase(
                        CleanPdfText(value),
                        font
                    )
                );


            cell.PaddingTop = 5;

            cell.PaddingBottom = 5;

            cell.PaddingLeft = 5;

            cell.PaddingRight = 5;


            cell.VerticalAlignment =
                Element.ALIGN_TOP;


            /*
             * Long banners wrap inside the cell.
             */

            cell.NoWrap =
                false;


            cell.UseAscender =
                true;

            cell.UseDescender =
                true;


            table.AddCell(
                cell
            );
        }


        // =========================================================
        // CLEAN PDF TEXT
        // =========================================================

        private string CleanPdfText(
            string value)
        {
            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return "-";
            }


            value =
                HttpUtility.HtmlDecode(
                    value
                );


            value =
                value.Replace(
                    "\0",
                    ""
                );


            value =
                value.Replace(
                    "\r",
                    " "
                );


            value =
                value.Replace(
                    "\n",
                    " "
                );


            return value.Trim();
        }


        // =========================================================
        // PORT RESULT CLASS
        // =========================================================

        private class PortResult
        {
            public int Port
            {
                get;
                set;
            }


            public string Status
            {
                get;
                set;
            }


            public string Version
            {
                get;
                set;
            }


            public long ResponseTime
            {
                get;
                set;
            }
        }
    }
}