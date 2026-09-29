# 🔎 NetScan Pro

> A web-based TCP port scanning and network reconnaissance tool built with ASP.NET Web Forms and C#.

[![C#](https://img.shields.io/badge/C%23-.NET-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-blue.svg)](https://dotnet.microsoft.com/apps/aspnet)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
[![Security](https://img.shields.io/badge/Purpose-Cybersecurity-red.svg)](#-responsible-use)

---

## 📌 Overview

**NetScan Pro** is a web-based TCP port scanning application designed for network security analysis and educational cybersecurity testing.

The application allows users to specify a target hostname or IP address and scan selected TCP ports. It analyzes the connection status of each port and provides additional information such as common service identification, banner/version information, response latency, and basic risk classification.

Scan results are displayed through a professional dashboard and can be exported as a detailed PDF security report.

---

## ✨ Features

### 🔍 TCP Port Scanning
- Scan specific TCP ports
- Support for multiple port values
- Fast asynchronous scanning
- Configurable connection timeout
- Concurrent scanning for improved performance

### 🌐 Target Resolution
- Hostname resolution
- IPv4 address detection
- Target information display
- TCP connectivity testing

### 🛠️ Service Detection
NetScan Pro identifies common services associated with scanned ports, including:

- FTP
- SSH
- SMTP
- HTTP
- HTTPS
- DNS
- MySQL
- Microsoft SQL Server
- PostgreSQL
- Redis
- MongoDB
- RDP
- VNC

### 🏷️ Banner & Version Detection

Where supported, the scanner attempts to retrieve service information through:

- HTTP banners
- HTTPS responses
- FTP banners
- SSH banners
- SMTP banners
- POP3 banners
- IMAP banners

This information can help security analysts understand what services are exposed on a target system.

### ⚠️ Risk Classification

Scanned ports are assigned a basic risk category:

| Risk | Description |
|------|-------------|
| 🔴 HIGH | Commonly sensitive or remotely accessible services |
| 🟠 MEDIUM | Services that may require additional security review |
| 🟢 LOW | Common web services |
| 🔵 INFO | Informational / unclassified service |

> Risk classification is a basic indicator and should not be treated as a complete vulnerability assessment.

### 📊 Security Dashboard

The dashboard provides:

- Total ports scanned
- Open ports
- Closed ports
- Timeout results
- Scan duration
- Target hostname
- Resolved IP address
- Port status
- Service information
- Banner/version information
- Response latency

### 📄 PDF Security Reports

NetScan Pro can generate a structured PDF report containing:

- Scan information
- Target details
- Scan summary
- Port results
- Open-port information
- Service identification
- Version/banner information
- Risk classification
- Response latency

---

## 🖥️ Interface

The application provides a clean cybersecurity-focused dashboard for performing scans and reviewing results.

### Target & Port Selection

Users can enter:

```text
Target: IP/WEB SITE 

Ports:
22,80,443,3306,3389
