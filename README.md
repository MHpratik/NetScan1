<div align="center">

# ⚡ NetScan1

### 🚀 Web-Based TCP Port Scanner & Network Security Analyzer

<img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=22&duration=3000&pause=800&color=00E5FF&center=true&vCenter=true&width=750&lines=Cybersecurity+Port+Scanner;TCP+Service+Detection;Banner+%26+Version+Detection;Risk+Analysis+%26+PDF+Reports" alt="Typing Animation" />

<br>

![GitHub repo size](https://img.shields.io/github/repo-size/ingal/NetScan1?style=for-the-badge&color=00e5ff)
![GitHub stars](https://img.shields.io/github/stars/ingal/NetScan1?style=for-the-badge&color=ffd700)
![GitHub forks](https://img.shields.io/github/forks/ingal/NetScan1?style=for-the-badge&color=9b59b6)
![GitHub license](https://img.shields.io/github/license/ingal/NetScan1?style=for-the-badge&color=00ff88)

<br>

**A lightweight cybersecurity tool for discovering open TCP ports, identifying services, analyzing banners, and generating professional scan reports.**

</div>

---

## 🖥️ About The Project

**NetScan1** is a web-based TCP port scanning application developed using **ASP.NET Web Forms and C#**.

The application allows a user to enter a target hostname or IP address and scan selected TCP ports. It determines whether each port is **Open, Closed, or Timed Out**, identifies commonly associated services, attempts to retrieve service banners/version information, and assigns a basic risk level.

The scan results are displayed through a modern dashboard and can be exported as a **professional PDF security report**.

> ⚠️ **For authorized security testing only. Scan systems and networks that you own or have explicit permission to test.**

---

# ✨ Features

<table>
<tr>
<td width="50%">

### 🔍 Port Scanning
- TCP port scanning
- Specific-port selection
- Hostname/IP support
- Configurable scan targets
- Connection timeout handling

</td>

<td width="50%">

### 🧠 Service Detection
- Common service identification
- TCP banner detection
- HTTP banner detection
- HTTPS banner detection
- Version information when available

</td>
</tr>

<tr>
<td>

### 🛡️ Security Analysis
- Open/Closed/Timeout classification
- Basic port risk classification
- High/Medium/Low risk levels
- Target information
- Response latency

</td>

<td>

### 📄 Reporting
- Professional PDF reports
- Scan summary
- Port results
- Service information
- Banner/version details
- Security risk information

</td>
</tr>
</table>

---

# ⚙️ How It Works

```text
                    ┌──────────────────┐
                    │   Target Host    │
                    │  IP / Hostname   │
                    └────────┬─────────┘
                             │
                             ▼
                  ┌─────────────────────┐
                  │   DNS Resolution    │
                  └──────────┬──────────┘
                             │
                             ▼
                  ┌─────────────────────┐
                  │   TCP Port Scan     │
                  └──────────┬──────────┘
                             │
               ┌─────────────┼─────────────┐
               ▼             ▼             ▼
          ┌────────┐    ┌─────────┐   ┌─────────┐
          │  OPEN  │    │ CLOSED  │   │ TIMEOUT │
          └────┬───┘    └─────────┘   └─────────┘
               │
               ▼
       ┌───────────────────┐
       │ Service Detection │
       └─────────┬─────────┘
                 │
                 ▼
       ┌───────────────────┐
       │ Banner / Version  │
       │     Detection     │
       └─────────┬─────────┘
                 │
                 ▼
       ┌───────────────────┐
       │    Risk Analysis  │
       └─────────┬─────────┘
                 │
                 ▼
       ┌───────────────────┐
       │ Dashboard Results │
       └─────────┬─────────┘
                 │
                 ▼
       ┌───────────────────┐
       │   PDF Report      │
       └───────────────────┘
