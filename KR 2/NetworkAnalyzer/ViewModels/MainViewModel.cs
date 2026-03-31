using NetworkAnalyzer.Data;
using NetworkAnalyzer.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace NetworkAnalyzer.ViewModels;



public class MainViewModel : ViewModelBase
{
    private readonly AppDbContext _context;
    private ObservableCollection<NetworkInterfaceInfo> _networkInterfaces = new();
    private NetworkInterfaceInfo? _selectedInterface;
    private string _urlInput = "";
    private string _analysisResult = "";
    private ObservableCollection<UrlHistory> _urlHistory = new();
    private string _pingResult = "";
    
    public ObservableCollection<NetworkInterfaceInfo> NetworkInterfaces
    {
        get => _networkInterfaces;
        set => SetProperty(ref _networkInterfaces, value);
    }
    
    public NetworkInterfaceInfo? SelectedInterface
    {
        get => _selectedInterface;
        set
        {
            if (SetProperty(ref _selectedInterface, value))
                OnPropertyChanged(nameof(SelectedInterfaceDetails));
        }
    }
    
    public string SelectedInterfaceDetails
    {
        get
        {
            if (SelectedInterface == null) return "Не выбран";
            return $"IP: {SelectedInterface.IpAddress}\n" +
                   $"Маска: {SelectedInterface.SubnetMask}\n" +
                   $"MAC: {SelectedInterface.MacAddress}\n" +
                   $"Статус: {SelectedInterface.Status}\n" +
                   $"Скорость: {SelectedInterface.SpeedFormatted}\n" +
                   $"Тип: {SelectedInterface.InterfaceType}";
        }
    }
    
    public string UrlInput
    {
        get => _urlInput;
        set => SetProperty(ref _urlInput, value);
    }
    
    public string AnalysisResult
    {
        get => _analysisResult;
        set => SetProperty(ref _analysisResult, value);
    }
    
    public ObservableCollection<UrlHistory> UrlHistory
    {
        get => _urlHistory;
        set => SetProperty(ref _urlHistory, value);
    }
    
    public string PingResult
    {
        get => _pingResult;
        set => SetProperty(ref _pingResult, value);
    }
    
    public ICommand AnalyzeUrlCommand { get; }
    public ICommand PingCommand { get; }
    public ICommand ClearHistoryCommand { get; }
    
    public MainViewModel()
    {
        _context = new AppDbContext();
        _context.Database.EnsureCreated();
        
        AnalyzeUrlCommand = new RelayCommand(_ => AnalyzeUrl());
        PingCommand = new RelayCommand(_ => PingHost());
        ClearHistoryCommand = new RelayCommand(_ => ClearHistory());
        
        LoadNetworkInterfaces();
        LoadUrlHistory();
    }
    
    private void LoadNetworkInterfaces()
    {
        var interfaces = new ObservableCollection<NetworkInterfaceInfo>();
        
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            var ipProps = ni.GetIPProperties();
            var ipAddress = "";
            var subnetMask = "";
            
            foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.Address.ToString();
                    subnetMask = ip.IPv4Mask?.ToString() ?? "";
                    break;
                }
            }
            
            var info = new NetworkInterfaceInfo
            {
                Name = ni.Name,
                Description = ni.Description,
                IpAddress = string.IsNullOrEmpty(ipAddress) ? "Нет IPv4" : ipAddress,
                SubnetMask = subnetMask,
                MacAddress = ni.GetPhysicalAddress().ToString(),
                Status = ni.OperationalStatus.ToString(),
                Speed = ni.Speed,
                InterfaceType = ni.NetworkInterfaceType.ToString()
            };
            
            interfaces.Add(info);
        }
        
        NetworkInterfaces = interfaces;
    }
    
    private void LoadUrlHistory()
    {
        _context.UrlHistories.Load();
        UrlHistory = new ObservableCollection<UrlHistory>(_context.UrlHistories.OrderByDescending(h => h.CheckTime));
    }
    
    private void AnalyzeUrl()
    {
        if (string.IsNullOrWhiteSpace(UrlInput))
        {
            AnalysisResult = "Введите URL для анализа";
            return;
        }
        
        try
        {
            var url = UrlInput.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;
            
            var uri = new Uri(url);
            
            var result = $"Анализ URL: {uri}\n\n";
            result += $"Схема (протокол): {uri.Scheme}\n";
            result += $"Хост: {uri.Host}\n";
            result += $"Порт: {uri.Port}\n";
            result += $"Путь: {uri.AbsolutePath}\n";
            result += $"Параметры запроса: {uri.Query}\n";
            result += $"Фрагмент: {uri.Fragment}\n";
            
            if (IPAddress.TryParse(uri.Host, out var ip))
            {
                if (IPAddress.IsLoopback(ip))
                    result += $"Тип адреса: Loopback (локальный)\n";
                else if (ip.ToString().StartsWith("10.") || 
                         ip.ToString().StartsWith("172.16.") || 
                         ip.ToString().StartsWith("192.168."))
                    result += $"Тип адреса: Локальный (частный)\n";
                else
                    result += $"Тип адреса: Публичный\n";
            }
            else
            {
                result += $"Тип адреса: Доменное имя\n";
            }
            AnalysisResult = result;
        }
        catch (Exception ex)
        {
            AnalysisResult = $"Ошибка парсинга URL: {ex.Message}";
        }
    }
    
    private async void PingHost()
    {
        if (string.IsNullOrWhiteSpace(UrlInput))
        {
            PingResult = "Введите URL или IP для проверки";
            return;
        }
        
        PingResult = "Проверка доступности...";
        
        try
        {
            var url = UrlInput.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;
            
            var uri = new Uri(url);
            var host = uri.Host;
            
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, 3000);
            
            var isAvailable = reply.Status == IPStatus.Success;
            var ipAddress = reply.Address?.ToString() ?? "Не определен";
            var hostName = "";
            
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(host);
                hostName = hostEntry.HostName;
            }
            catch { hostName = host; }
            
            PingResult = isAvailable 
                ? $" {host} доступен\nIP: {ipAddress}\nВремя: {reply.RoundtripTime} мс" 
                : $" {host} недоступен\nСтатус: {reply.Status}";
            
            var history = new UrlHistory
            {
                Url = url,
                CheckTime = DateTime.Now,
                IsAvailable = isAvailable,
                IpAddress = ipAddress,
                HostName = hostName
            };
            
            _context.UrlHistories.Add(history);
            _context.SaveChanges();
            LoadUrlHistory();
        }
        catch (Exception ex)
        {
            PingResult = $"Ошибка проверки: {ex.Message}";
            var history = new UrlHistory
            {
                Url = UrlInput,
                CheckTime = DateTime.Now,
                IsAvailable = false,
                IpAddress = null,
                HostName = null
            };
            
            _context.UrlHistories.Add(history);
            _context.SaveChanges();
            LoadUrlHistory();
        }
    }
    
    private void ClearHistory()
    {
        _context.UrlHistories.RemoveRange(_context.UrlHistories);
        _context.SaveChanges();
        LoadUrlHistory();
    }
}