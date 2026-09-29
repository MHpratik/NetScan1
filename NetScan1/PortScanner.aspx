<%@ Page Language="C#"
    Async="true"
    AutoEventWireup="true"
    CodeBehind="PortScanner.aspx.cs"
    Inherits="PortScanner.PortScanner" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>NetScan Pro - Advanced Port Scanner</title>

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <style>

        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 0;
            background: #070b14;
            color: #e5e7eb;
            font-family: "Segoe UI", Arial, sans-serif;
        }

        .navbar {
            height: 70px;
            background: #0c1220;
            border-bottom: 1px solid #1e293b;

            display: flex;
            align-items: center;
            justify-content: space-between;

            padding: 0 35px;
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 12px;

            font-size: 21px;
            font-weight: 700;
        }

        .brand-icon {
            width: 42px;
            height: 42px;

            display: flex;
            align-items: center;
            justify-content: center;

            border-radius: 10px;

            background: #2563eb;

            box-shadow:
                0 0 20px rgba(37, 99, 235, .35);

            font-size: 20px;
        }

        .brand-blue {
            color: #60a5fa;
        }

        .online {
            color: #94a3b8;
            font-size: 13px;
        }

        .online-dot {
            display: inline-block;

            width: 9px;
            height: 9px;

            background: #22c55e;

            border-radius: 50%;

            margin-right: 7px;

            box-shadow:
                0 0 10px #22c55e;
        }

        .container {
            width: 94%;
            max-width: 1450px;

            margin: 30px auto;
        }

        .hero {
            margin-bottom: 25px;
        }

        .hero h1 {
            color: #f8fafc;

            font-size: 32px;

            margin: 0 0 7px 0;
        }

        .hero p {
            color: #64748b;

            font-size: 14px;
        }

        .card {
            background: #0d1422;

            border: 1px solid #1e293b;

            border-radius: 15px;

            padding: 25px;

            margin-bottom: 22px;

            box-shadow:
                0 15px 40px rgba(0,0,0,.15);
        }

        .card-title {
            color: #f8fafc;

            font-size: 16px;

            font-weight: 600;

            margin-bottom: 20px;
        }

        .field {
            width: 100%;
        }

        .field label {
            display: block;

            color: #94a3b8;

            font-size: 11px;

            margin-bottom: 8px;

            text-transform: uppercase;
        }

        .input {
            width: 100%;

            height: 46px;

            background: #080e19;

            border: 1px solid #293548;

            border-radius: 8px;

            color: white;

            padding: 0 13px;

            outline: none;

            font-size: 14px;
        }

        .input:focus {
            border-color: #3b82f6;

            box-shadow:
                0 0 0 3px rgba(59,130,246,.10);
        }

        .help {
            color: #64748b;

            font-size: 11px;

            margin-top: 8px;
        }

        .button-row {
            display: flex;

            gap: 10px;

            margin-top: 20px;

            flex-wrap: wrap;
        }

        .btn {
            min-height: 45px;

            border: none;

            border-radius: 8px;

            padding: 0 20px;

            cursor: pointer;

            font-weight: 600;

            color: white;

            font-size: 12px;
        }

        .btn-primary {
            background: #2563eb;

            flex: 1;
        }

        .btn-primary:hover {
            background: #1d4ed8;
        }

        .btn-danger {
            background: #7f1d1d;
        }

        .btn-danger:hover {
            background: #991b1b;
        }

        .btn-export {
            background: #17243a;

            border: 1px solid #293548;
        }

        .btn-export:hover {
            border-color: #3b82f6;
        }

        .btn-pdf {
            background: #991b1b;

            flex: 1;
        }

        .btn-pdf:hover {
            background: #b91c1c;
        }

        .message {
            display: block;

            background: #101a2b;

            border: 1px solid #26364f;

            color: #93c5fd;

            border-radius: 8px;

            padding: 12px;

            margin-top: 18px;

            font-size: 13px;
        }

        .progress-area {
            margin-top: 20px;
        }

        .progress-top {
            display: flex;

            justify-content: space-between;

            color: #64748b;

            font-size: 12px;

            margin-bottom: 8px;
        }

        .progress-background {
            height: 7px;

            background: #111827;

            border-radius: 10px;

            overflow: hidden;
        }

        .progress-bar {
            height: 100%;

            width: 0%;

            background: #2563eb;

            transition: width .2s;
        }

        .stats {
            display: grid;

            grid-template-columns:
                repeat(5, 1fr);

            gap: 14px;

            margin-bottom: 22px;
        }

        .stat {
            background: #0d1422;

            border: 1px solid #1e293b;

            border-radius: 12px;

            padding: 18px;
        }

        .stat-label {
            color: #64748b;

            font-size: 10px;

            text-transform: uppercase;

            margin-bottom: 8px;
        }

        .stat-number {
            font-size: 25px;

            font-weight: 700;

            color: #f8fafc;
        }

        .blue {
            color: #60a5fa;
        }

        .green {
            color: #22c55e;
        }

        .red {
            color: #ef4444;
        }

        .yellow {
            color: #f59e0b;
        }

        .target-grid {
            display: grid;

            grid-template-columns:
                repeat(4, 1fr);

            gap: 20px;
        }

        .target-item label {
            display: block;

            color: #64748b;

            font-size: 10px;

            text-transform: uppercase;

            margin-bottom: 6px;
        }

        .target-item div {
            color: #e2e8f0;

            font-size: 13px;

            font-weight: 600;

            overflow-wrap: anywhere;
        }

        .results-card {
            background: #0d1422;

            border: 1px solid #1e293b;

            border-radius: 15px;

            overflow: hidden;
        }

        .results-header {
            padding: 19px 22px;

            border-bottom:
                1px solid #1e293b;

            display: flex;

            justify-content: space-between;

            align-items: center;

            gap: 15px;
        }

        .results-title {
            font-size: 16px;

            font-weight: 600;
        }

        .filter {
            width: 280px;

            height: 36px;

            background: #080e19;

            border: 1px solid #293548;

            color: white;

            border-radius: 7px;

            padding: 0 10px;

            outline: none;
        }

        .table-wrapper {
            overflow-x: auto;
        }

        .results-table {
            width: 100%;

            border-collapse: collapse;
        }

        .results-table th {
            background: #09101d;

            color: #64748b;

            padding: 13px 18px;

            text-align: left;

            font-size: 10px;

            white-space: nowrap;
        }

        .results-table td {
            padding: 13px 18px;

            border-top:
                1px solid #172131;

            color: #cbd5e1;

            font-size: 12px;

            vertical-align: top;
        }

        .results-table tr:hover td {
            background: #101a2a;
        }

        .open-badge {
            display: inline-block;

            color: #4ade80;

            background:
                rgba(34,197,94,.1);

            border:
                1px solid rgba(34,197,94,.3);

            padding: 4px 8px;

            border-radius: 5px;

            font-size: 10px;

            font-weight: 700;
        }

        .closed-badge {
            display: inline-block;

            color: #f87171;

            background:
                rgba(239,68,68,.08);

            border:
                1px solid rgba(239,68,68,.2);

            padding: 4px 8px;

            border-radius: 5px;

            font-size: 10px;

            font-weight: 700;
        }

        .timeout-badge {
            display: inline-block;

            color: #fbbf24;

            background:
                rgba(245,158,11,.08);

            border:
                1px solid rgba(245,158,11,.2);

            padding: 4px 8px;

            border-radius: 5px;

            font-size: 10px;

            font-weight: 700;
        }

        .risk-high {
            color: #f87171;

            font-weight: 700;
        }

        .risk-medium {
            color: #fbbf24;

            font-weight: 700;
        }

        .risk-low {
            color: #4ade80;

            font-weight: 700;
        }

        .risk-info {
            color: #60a5fa;

            font-weight: 600;
        }

        .version {
            display: block;

            color: #93c5fd;

            max-width: 450px;

            white-space: normal;

            overflow-wrap: anywhere;

            line-height: 1.5;
        }

        .footer {
            text-align: center;

            color: #475569;

            font-size: 11px;

            padding: 30px;

            line-height: 1.8;
        }

        @media(max-width: 1000px) {

            .stats {
                grid-template-columns:
                    repeat(3, 1fr);
            }

            .target-grid {
                grid-template-columns:
                    repeat(2, 1fr);
            }
        }

        @media(max-width: 650px) {

            .navbar {
                padding: 0 15px;
            }

            .container {
                width: 94%;
            }

            .stats {
                grid-template-columns:
                    1fr 1fr;
            }

            .target-grid {
                grid-template-columns:
                    1fr;
            }

            .results-header {
                flex-direction: column;

                align-items: stretch;
            }

            .filter {
                width: 100%;
            }

            .button-row {
                flex-direction: column;
            }

            .btn-primary,
            .btn-pdf {
                width: 100%;
            }
        }

    </style>


    <script type="text/javascript">

        function filterResults() {

            var input =
                document.getElementById(
                    "txtFilter"
                ).value.toLowerCase();


            var table =
                document.getElementById(
                    "gvResults"
                );


            if (!table) {
                return;
            }


            var rows =
                table.getElementsByTagName(
                    "tr"
                );


            for (
                var i = 1;
                i < rows.length;
                i++
            ) {

                var text =
                    rows[i].innerText
                        .toLowerCase();


                if (
                    text.indexOf(input) >= 0
                ) {

                    rows[i].style.display =
                        "";

                }
                else {

                    rows[i].style.display =
                        "none";

                }

            }

        }

    </script>

</head>


<body>

<form
    id="form1"
    runat="server">


    <!-- ================================================= -->
    <!-- NAVIGATION -->
    <!-- ================================================= -->

    <div class="navbar">

        <div class="brand">

            <div class="brand-icon">
                ◈
            </div>

            <div>

                NetScan

                <span class="brand-blue">
                    Pro
                </span>

            </div>

        </div>


        <div class="online">

            <span class="online-dot"></span>

            Scanner Ready

        </div>

    </div>


    <div class="container">


        <!-- ================================================= -->
        <!-- HERO -->
        <!-- ================================================= -->

        <div class="hero">

            <h1>
                Advanced TCP Port Scanner
            </h1>

            <p>
                TCP connectivity, service discovery
                and banner/version detection.
            </p>

        </div>


        <!-- ================================================= -->
        <!-- CONFIGURATION -->
        <!-- ================================================= -->

        <div class="card">

            <div class="card-title">
                Scan Configuration
            </div>


            <div class="field">

                <label>
                    Target Host / IP Address
                </label>


                <asp:TextBox
                    ID="txtTarget"
                    runat="server"
                    CssClass="input"
                    placeholder="127.0.0.1 or authorized-host.example">
                </asp:TextBox>

            </div>


            <br />


            <div class="field">

                <label>
                    Ports To Scan
                </label>


                <asp:TextBox
                    ID="txtPorts"
                    runat="server"
                    CssClass="input"
                    placeholder="22,80,443,3306,3389">
                </asp:TextBox>


                <div class="help">

                    Enter individual TCP ports
                    separated by commas.

                    Example:
                    22,80,443,3306,3389

                </div>

            </div>


            <div class="button-row">

                <asp:Button
                    ID="btnScan"
                    runat="server"
                    Text="START SCAN"
                    CssClass="btn btn-primary"
                    OnClick="btnScan_Click" />


                <asp:Button
                    ID="btnCancel"
                    runat="server"
                    Text="CANCEL"
                    CssClass="btn btn-danger"
                    OnClick="btnCancel_Click"
                    CausesValidation="false" />

            </div>


            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message"
                Visible="false">
            </asp:Label>


            <div class="progress-area">

                <div class="progress-top">

                    <span>
                        Scan Progress
                    </span>


                    <asp:Label
                        ID="lblProgress"
                        runat="server"
                        Text="0%">
                    </asp:Label>

                </div>


                <div class="progress-background">

                    <div
                        id="progressBar"
                        runat="server"
                        class="progress-bar">
                    </div>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- STATISTICS -->
        <!-- ================================================= -->

        <div class="stats">


            <div class="stat">

                <div class="stat-label">
                    Ports Scanned
                </div>

                <div class="stat-number blue">

                    <asp:Label
                        ID="lblScanned"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </div>

            </div>


            <div class="stat">

                <div class="stat-label">
                    Open Ports
                </div>

                <div class="stat-number green">

                    <asp:Label
                        ID="lblOpen"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </div>

            </div>


            <div class="stat">

                <div class="stat-label">
                    Closed
                </div>

                <div class="stat-number red">

                    <asp:Label
                        ID="lblClosed"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </div>

            </div>


            <div class="stat">

                <div class="stat-label">
                    Timeout
                </div>

                <div class="stat-number yellow">

                    <asp:Label
                        ID="lblTimeout"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </div>

            </div>


            <div class="stat">

                <div class="stat-label">
                    Scan Time
                </div>

                <div class="stat-number">

                    <asp:Label
                        ID="lblScanTime"
                        runat="server"
                        Text="0 ms">
                    </asp:Label>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- TARGET INFORMATION -->
        <!-- ================================================= -->

        <div class="card">

            <div class="card-title">
                Target Information
            </div>


            <div class="target-grid">


                <div class="target-item">

                    <label>
                        Host
                    </label>

                    <div>

                        <asp:Label
                            ID="lblTarget"
                            runat="server"
                            Text="-">
                        </asp:Label>

                    </div>

                </div>


                <div class="target-item">

                    <label>
                        Resolved IP
                    </label>

                    <div>

                        <asp:Label
                            ID="lblResolvedIP"
                            runat="server"
                            Text="-">
                        </asp:Label>

                    </div>

                </div>


                <div class="target-item">

                    <label>
                        Protocol
                    </label>

                    <div>
                        TCP
                    </div>

                </div>


                <div class="target-item">

                    <label>
                        Ports
                    </label>

                    <div>

                        <asp:Label
                            ID="lblPortList"
                            runat="server"
                            Text="-">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- RESULTS -->
        <!-- ================================================= -->

        <div class="results-card">

            <div class="results-header">

                <div class="results-title">
                    Detected Services
                </div>


                <input
                    id="txtFilter"
                    type="text"
                    class="filter"
                    placeholder="Search results..."
                    onkeyup="filterResults();" />

            </div>


            <div class="table-wrapper">


                <asp:GridView
                    ID="gvResults"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="results-table"
                    GridLines="None"
                    ShowHeader="true"
                    EmptyDataText="No results available."
                    ClientIDMode="Static">


                    <Columns>


                        <asp:BoundField
                            DataField="Port"
                            HeaderText="PORT" />


                        <asp:TemplateField
                            HeaderText="STATUS">

                            <ItemTemplate>

                                <span
                                    class='<%# GetStatusClass(Convert.ToString(Eval("Status"))) %>'>

                                    <%# Eval("Status") %>

                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:BoundField
                            DataField="Service"
                            HeaderText="SERVICE" />


                        <asp:TemplateField
                            HeaderText="VERSION / BANNER">

                            <ItemTemplate>

                                <span class="version">

                                    <%#
                                        Server.HtmlEncode(
                                            Convert.ToString(
                                                Eval("Version")
                                            )
                                        )
                                    %>

                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:TemplateField
                            HeaderText="RISK">

                            <ItemTemplate>

                                <span
                                    class='<%# GetRiskClass(Convert.ToString(Eval("Risk"))) %>'>

                                    <%# Eval("Risk") %>

                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:BoundField
                            DataField="Response"
                            HeaderText="LATENCY" />


                    </Columns>

                </asp:GridView>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- REPORT BUTTONS -->
        <!-- ================================================= -->

        <div class="button-row">

            <asp:Button
                ID="btnPDF"
                runat="server"
                Text="GENERATE PDF REPORT"
                CssClass="btn btn-pdf"
                OnClick="btnPDF_Click"
                CausesValidation="false" />


            <asp:Button
                ID="btnClear"
                runat="server"
                Text="CLEAR RESULTS"
                CssClass="btn btn-export"
                OnClick="btnClear_Click"
                CausesValidation="false" />

        </div>


        <!-- ================================================= -->
        <!-- FOOTER -->
        <!-- ================================================= -->

        <div class="footer">

            NetScan Pro · ASP.NET Web Forms

            <br />

            Version information is reported only
            when exposed by the target service.

            <br />

            Use only on systems you own or are
            authorized to test.

        </div>


    </div>

</form>

</body>

</html>