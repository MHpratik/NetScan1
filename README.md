<div align="center">

# ⚡ NetScan1

### 🛡️ Advanced Web-Based TCP Port Scanner

<img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=22&duration=3000&pause=800&color=00FF9C&center=true&vCenter=true&width=700&lines=Scan+TCP+Ports+%F0%9F%94%8D;Detect+Running+Services+%F0%9F%9B%A1%EF%B8%8F;Analyze+Security+Risks+%E2%9A%A0%EF%B8%8F;Generate+Professional+PDF+Reports+%F0%9F%93%84" />

<br>

![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Backend-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Security](https://img.shields.io/badge/Cybersecurity-Tool-00FF9C?style=for-the-badge&logo=hackthebox&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)

<br>

**🔎 Scan • 🧠 Analyze • 🛡️ Detect • 📊 Report**

</div>

---

## 🚀 About NetScan1

**NetScan1** is a web-based cybersecurity port scanning application developed using **ASP.NET Web Forms and C#**.

The application allows users to enter a target hostname or IP address and scan selected TCP ports to determine their current state.

NetScan1 can identify:

- 🟢 Open ports
- 🔴 Closed ports
- 🟡 Timeout ports
- 🔍 Common services
- 🧾 Service banners and version information
- ⚠️ Basic security risk levels
- ⏱️ Port response latency
- 🌐 Target IP resolution
- 📄 Professional PDF reports

> ⚠️ **Use this tool only against systems you own or have explicit permission to test.**

---

# ✨ Features

<table>
<tr>
<td width="50%">

### 🔍 Port Scanning
- TCP port scanning
- Custom port selection
- Multiple ports at once
- Connection timeout handling
- Concurrent scanning

</td>

<td width="50%">

### 🧠 Service Detection
- Common service identification
- Banner grabbing
- Version detection
- HTTP/HTTPS information
- TCP service analysis

</td>
</tr>

<tr>
<td>

### 🛡️ Security Analysis
- Risk classification
- High-risk port detection
- Medium-risk port detection
- Low-risk port detection
- Informational results

</td>

<td>

### 📊 Reporting
- Live scan statistics
- Scan duration
- Response latency
- Detailed port results
- PDF report generation

</td>
</tr>
</table>

---

# 🎯 How It Works

```text
             ┌─────────────────────┐
             │      TARGET         │
             │  IP / Hostname      │
             └──────────┬──────────┘
                        │
                        ▼
             ┌─────────────────────┐
             │   DNS Resolution    │
             └──────────┬──────────┘
                        │
                        ▼
             ┌─────────────────────┐
             │    TCP Scanner      │
             │  Selected Ports     │
             └──────────┬──────────┘
                        │
             ┌──────────┴──────────┐
             ▼                     ▼
       ┌───────────┐        ┌──────────────┐
       │ Connection│        │   Timeout    │
       │ Successful│        │              │
       └─────┬─────┘        └──────┬───────┘
             │                     │
             ▼                     ▼
       ┌────────────┐        ┌────────────┐
       │   Service  │        │   Timeout  │
       │  Detection │        │   Result   │
       └─────┬──────┘        └────────────┘
             │
             ▼
       ┌────────────┐
       │ Risk Level │
       │  Analysis  │
       └─────┬──────┘
             │
             ▼
       ┌────────────┐
       │ Dashboard  │
       └─────┬──────┘
             │
             ▼
       ┌────────────┐
       │ PDF Report │
       └────────────┘
