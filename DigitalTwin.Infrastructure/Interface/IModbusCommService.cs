using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalTwin.Infrastructure.Models;

namespace DigitalTwin.Infrastructure.Interface
{
    public interface IModbusCommService
    {
        // ── 状态 ──────────────────────────────────────────
        bool IsConnected { get; }
        string Host { get; }
        int Port { get; }
        long TotalRequests { get; }
        long FailedRequests { get; }
        double PacketLossRate { get; }

        // ── 事件 ──────────────────────────────────────────
        event Action<bool>? OnConnectionChanged;
        event Action<string>? OnError;

        // ── 连接控制 ──────────────────────────────────────
        Task<bool> ConnectAsync(string host, int port = 502);
        void Disconnect();

        // ── 读取 ──────────────────────────────────────────
        Task<int[]?> ReadHoldingRegistersAsync(int startAddress, int count);
        Task<int[]?> ReadInputRegistersAsync(int startAddress, int count);
        Task<bool[]?> ReadCoilsAsync(int startAddress, int count);

        // ── 写入 ──────────────────────────────────────────
        Task<bool> WriteSingleRegisterAsync(int address, int value);
        Task<bool> WriteMultipleRegistersAsync(int startAddress, int[] values);
        Task<bool> WriteSingleCoilAsync(int address, bool value);
        Task<bool> WriteAndVerifyAsync(int address, int value);

        // ── 轮询 ──────────────────────────────────────────
        void StartPolling(int intervalMs = 100);
        void StopPolling();

        // ── 快照 ──────────────────────────────────────────
        SrmDeviceData? GetDeviceSnapshot(string deviceId);
    }
}
