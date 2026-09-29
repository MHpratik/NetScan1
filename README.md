<div align="center">

# ⚡ NETSCAN1

### `CYBERSECURITY • NETWORK RECONNAISSANCE • TCP ANALYSIS`

<img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=20&duration=2500&pause=700&color=00FF9C&center=true&vCenter=true&width=800&lines=Initializing+NetScan1...;Target+Detection+%5BOK%5D;TCP+Port+Scanning+%5BOK%5D;Service+Fingerprinting+%5BOK%5D;Banner+Analysis+%5BOK%5D;Security+Report+Generation+%5BOK%5D" />

<br>

![C#](https://img.shields.io/badge/C%23-ASP.NET-512BD4?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-0A66C2?style=for-the-badge)
![Security](https://img.shields.io/badge/Cybersecurity-Network%20Scanner-00FF9C?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)

<br>

### 🔴 `SYSTEM STATUS: ONLINE`
### 🟢 `SCANNER STATUS: READY`

</div>

---

# 🖥️ NETSCAN1

> **A modern web-based TCP port scanner designed for network reconnaissance, service identification, banner detection, risk classification, and security reporting.**

NetScan1 is an **ASP.NET Web Forms cybersecurity application** developed using **C#**.

It allows authorized users to enter a hostname/IP address and scan selected TCP ports. The application analyzes the response of each port and displays detailed information through a professional security dashboard.

---

# ⚡ SYSTEM OVERVIEW

```text
                 ┌─────────────────────────┐
                 │       NETSCAN1           │
                 │  NETWORK SECURITY TOOL   │
                 └────────────┬────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │   TARGET INPUT   │
                    │ IP / HOSTNAME     │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │  DNS RESOLUTION  │
                    └────────┬─────────┘
                             │
                             ▼
                  ┌───────────────────────┐
                  │     TCP SCANNER       │
                  └───────────┬───────────┘
                              │
               ┌──────────────┼──────────────┐
               │              │              │
               ▼              ▼              ▼
          ┌─────────┐    ┌─────────┐    ┌─────────┐
          │  OPEN   │    │ CLOSED  │    │ TIMEOUT │
          └────┬────┘    └─────────┘    └─────────┘
               │
               ▼
      ┌─────────────────────┐
      │ SERVICE IDENTIFIER   │
      └──────────┬──────────┘
                 │
                 ▼
      ┌─────────────────────┐
      │ BANNER / VERSION     │
      │     DETECTION        │
      └──────────┬──────────┘
                 │
                 ▼
      ┌─────────────────────┐
      │    RISK ANALYSIS    │
      └──────────┬──────────┘
                 │
                 ▼
      ┌─────────────────────┐
      │ SECURITY DASHBOARD  │
      └──────────┬──────────┘
                 │
                 ▼
      ┌─────────────────────┐
      │     PDF REPORT      │
      └─────────────────────┘
