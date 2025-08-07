using System;
using System.ComponentModel;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Http;

[Route("/")]
public sealed class HelloController : ControllerBase
{
    private static bool _shownHello = false;
    private readonly ILogger<HelloController> _logger;
    private readonly IGameManager _gameManager;

    public HelloController(ILogger<HelloController> logger, IGameManager gameManager)
    {
        _logger = logger;
        _gameManager = gameManager;
    }

    [HttpGet]
    public IActionResult GetHello()
    {
        if (!_shownHello)
        {
            _shownHello = true;
            _logger.LogInformation("Impostor's web status page is reachable (this message is only printed once per start)");
        }

        var serverVersion = DotnetUtils.Version;
        var totalGames = _gameManager.Games.Count();
        var totalPlayers = _gameManager.Games.Sum(g => g.PlayerCount);

        var html = $@"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title data-i18n=""title"">Impostor Server Status</title>
            <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
            <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
            <link href=""https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&family=Roboto:wght@300;400;500&display=swap"" rel=""stylesheet"">
            <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"">
            <style>
                :root {{
                    --primary: #3498db;
                    --primary-dark: #2980b9;
                    --danger: #e74c3c;
                    --success: #2ecc71;
                    --warning: #f39c12;
                    --dark: #2c3e50;
                    --light: #ecf0f1;
                    --gray: #95a5a6;
                    --card-bg: #ffffff;
                    --body-bg: #f8f9fa;
                    --text: #34495e;
                    --text-light: #7f8c8d;
                    --shadow: 0 4px 12px rgba(0,0,0,0.08);
                    --radius: 12px;
                    --transition: all 0.3s ease;
                }}

                [data-theme=""dark""] {{
                    --card-bg: #2d3748;
                    --body-bg: #1a202c;
                    --text: #e2e8f0;
                    --text-light: #a0aec0;
                    --shadow: 0 4px 12px rgba(0,0,0,0.25);
                }}

                * {{
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }}

                body {{
                    font-family: 'Roboto', sans-serif;
                    background-color: var(--body-bg);
                    color: var(--text);
                    line-height: 1.6;
                    padding: 20px;
                    transition: var(--transition);
                }}

                .container {{
                    max-width: 1200px;
                    margin: 0 auto;
                }}

                header {{
                    background: linear-gradient(135deg, var(--primary), var(--primary-dark));
                    color: white;
                    padding: 30px 40px;
                    border-radius: var(--radius);
                    box-shadow: var(--shadow);
                    margin-bottom: 30px;
                    position: relative;
                    overflow: hidden;
                }}

                header::before {{
                    content: '';
                    position: absolute;
                    top: -50%;
                    right: -50%;
                    width: 200%;
                    height: 200%;
                    background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0) 70%);
                    pointer-events: none;
                }}

                h1, h2, h3 {{
                    font-family: 'Montserrat', sans-serif;
                    font-weight: 700;
                }}

                header h1 {{
                    font-size: 2.8rem;
                    margin-bottom: 10px;
                    display: flex;
                    align-items: center;
                    gap: 15px;
                }}

                header p {{
                    font-size: 1.2rem;
                    opacity: 0.9;
                    max-width: 600px;
                }}

                .server-stats {{
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
                    gap: 20px;
                    margin-bottom: 30px;
                }}

                .stat-card {{
                    background: var(--card-bg);
                    border-radius: var(--radius);
                    padding: 25px;
                    box-shadow: var(--shadow);
                    transition: var(--transition);
                    display: flex;
                    flex-direction: column;
                }}

                .stat-card:hover {{
                    transform: translateY(-5px);
                    box-shadow: 0 8px 20px rgba(0,0,0,0.12);
                }}

                .stat-icon {{
                    width: 60px;
                    height: 60px;
                    border-radius: 50%;
                    background: linear-gradient(135deg, var(--primary), var(--primary-dark));
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin-bottom: 20px;
                    font-size: 1.8rem;
                    color: white;
                }}

                .stat-value {{
                    font-size: 2.2rem;
                    font-weight: 700;
                    margin: 5px 0;
                    color: var(--primary);
                }}

                .stat-value.truncate {{white - space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    max-width: 100%;
                    display: block;
                }}

                .stat-label {{
                    color: var(--text-light);
                    font-size: 1rem;
                }}

                .section-title {{
                    display: flex;
                    align-items: center;
                    gap: 10px;
                    margin: 30px 0 20px;
                    font-size: 1.8rem;
                    color: var(--text);
                }}

                .game-table {{
                    width: 100%;
                    border-collapse: separate;
                    border-spacing: 0;
                    background: var(--card-bg);
                    border-radius: var(--radius);
                    overflow: hidden;
                    box-shadow: var(--shadow);
                }}

                .game-table th {{
                    background: linear-gradient(to right, var(--primary), var(--primary-dark));
                    color: white;
                    text-align: left;
                    padding: 18px 25px;
                    font-weight: 600;
                    font-family: 'Montserrat', sans-serif;
                }}

                .game-table td {{
                    padding: 16px 25px;
                    border-bottom: 1px solid rgba(0,0,0,0.05);
                }}

                .game-table tr:last-child td {{
                    border-bottom: none;
                }}

                .game-table tr:hover td {{
                    background-color: rgba(52, 152, 219, 0.05);
                }}

                .status {{
                    display: inline-block;
                    padding: 6px 14px;
                    border-radius: 20px;
                    font-size: 0.85rem;
                    font-weight: 500;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                }}

                .host-name {{
                    max-width: 150px;
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    display: inline-block;
                    vertical-align: middle;
                }}

                @media (max-width: 768px) {{
                    .host-name {{
                        max-width: 100px;
                    }}
                }}

                .status-lobby {{
                    background-color: rgba(46, 204, 113, 0.15);
                    color: var(--success);
                }}

                .status-ingame {{
                    background-color: rgba(231, 76, 60, 0.15);
                    color: var(--danger);
                }}

                .map-icon {{
                    display: inline-flex;
                    align-items: center;
                    gap: 8px;
                }}

                .empty-state {{
                    text-align: center;
                    padding: 50px 20px;
                    color: var(--text-light);
                    background: var(--card-bg);
                    border-radius: var(--radius);
                    box-shadow: var(--shadow);
                }}

                .empty-state i {{
                    font-size: 4rem;
                    margin-bottom: 20px;
                    opacity: 0.3;
                }}

                .theme-toggle {{
                    position: absolute;
                    top: 25px;
                    right: 25px;
                    background: rgba(255,255,255,0.15);
                    border: none;
                    width: 45px;
                    height: 45px;
                    border-radius: 50%;
                    color: white;
                    cursor: pointer;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 1.2rem;
                    transition: var(--transition);
                }}

                .language-toggle {{
                    position: absolute;
                    top: 25px;
                    right: 80px;
                    background: rgba(255,255,255,0.15);
                    border: none;
                    width: 45px;
                    height: 45px;
                    border-radius: 50%;
                    color: white;
                    cursor: pointer;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 1.2rem;
                    transition: var(--transition);
                }}

                .theme-toggle:hover, .language-toggle:hover {{
                    background: rgba(255,255,255,0.25);
                    transform: rotate(20deg);
                }}

                .footer {{
                    text-align: center;
                    margin-top: 40px;
                    color: var(--text-light);
                    font-size: 0.9rem;
                }}

                @media (max-width: 768px) {{
                    .server-stats {{
                        grid-template-columns: 1fr;
                    }}
                    
                    .game-table {{
                        display: block;
                        overflow-x: auto;
                    }}
                    
                    header h1 {{
                        font-size: 2rem;
                    }}
                    
                    .theme-toggle, .language-toggle {{
                        top: 15px;
                        right: 15px;
                    }}
                    
                    .language-toggle {{
                        right: 70px;
                    }}
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <header>
                    <button class=""theme-toggle"" id=""themeToggle"" title=""Toggle theme"">
                        <i class=""fas fa-moon""></i>
                    </button>
                    <button class=""language-toggle"" id=""languageToggle"" title=""Switch language"">
                        <i class=""fas fa-globe""></i>
                    </button>
                    <h1>
                        <i class=""fas fa-server""></i>
                        <span data-i18n=""headerTitle"">Impostor Console</span>
                    </h1>
                    <p data-i18n=""headerSubtitle"">Impostor Status & Management Panel</p>
                </header>

                <div class=""server-stats"">
                <div class=""stat-card"">
                    <div class=""stat-icon"">
                        <i class=""fas fa-code-branch""></i>
                    </div>
                    <div class=""stat-value truncate"" title=""{serverVersion}"">{serverVersion}</div>
                    <div class=""stat-label"" data-i18n=""serverVersion"">Server Version</div>
                </div>
    
                <div class=""stat-card"">
                    <div class=""stat-icon"">
                        <i class=""fas fa-network-wired""></i>
                    </div>
                    <div class=""stat-value truncate"" title=""{Request.Host}"">{Request.Host}</div>
                    <div class=""stat-label"" data-i18n=""serverAddress"">Server Address</div>
                </div>
                    <div class=""stat-card"">
                        <div class=""stat-icon"">
                            <i class=""fas fa-door-open""></i>
                        </div>
                        <div class=""stat-value"">{totalGames}</div>
                        <div class=""stat-label"" data-i18n=""activeRooms"">Active Rooms</div>
                    </div>
                    
                    <div class=""stat-card"">
                        <div class=""stat-icon"">
                            <i class=""fas fa-users""></i>
                        </div>
                        <div class=""stat-value"">{totalPlayers}</div>
                        <div class=""stat-label"" data-i18n=""onlinePlayers"">Online Players</div>
                    </div>
                </div>

                <h2 class=""section-title"">
                    <i class=""fas fa-list""></i>
                    <span data-i18n=""roomList"">Room List</span>
                </h2>

                {(totalGames == 0 ?
                    $@"<div class=""empty-state"">
                        <i class=""fas fa-gamepad""></i>
                        <h3 data-i18n=""noRoomsTitle"">No Active Rooms</h3>
                        <p data-i18n=""noRoomsMessage"">There are no active game rooms on the server currently</p>
                    </div>"
                    :
                    $@"<table class=""game-table"">
                        <thead>
                            <tr>
                                <th data-i18n=""roomCode"">Room Code</th>
                                <th data-i18n=""status"">Status</th>
                                <th data-i18n=""players"">Players</th>
                                <th data-i18n=""map"">Map</th>
                                <th data-i18n=""impostors"">Impostors</th>
                                <th data-i18n=""host"">Host</th>
                            </tr>
                        </thead>
                        <tbody>
                            {string.Join("", _gameManager.Games.Select(GameToHtmlRow))}
                        </tbody>
                    </table>"
                )}

                <div class=""footer"">
                    <p data-i18n=""footer"">Impostor © {DateTime.Now.Year} - Unofficial Among Us Server Implementation</p>
                </div>
            </div>

            <script>
                const i18n = {{
                    en: {{
                        title: ""Impostor Server Status"",
                        headerTitle: ""Impostor Server Console"",
                        headerSubtitle: ""Impostor Status & Management Panel"",
                        serverVersion: ""Server Version"",
                        activeRooms: ""Active Rooms"",
                        onlinePlayers: ""Online Players"",
                        serverAddress: ""Server Address"",
                        roomList: ""Room List"",
                        roomCode: ""Room Code"",
                        status: ""Status"",
                        players: ""Players"",
                        map: ""Map"",
                        impostors: ""Impostors"",
                        host: ""Host"",
                        noRoomsTitle: ""No Active Rooms"",
                        noRoomsMessage: ""There are no active game rooms on the server currently"",
                        footer: ""Impostor © {DateTime.Now.Year} - Unofficial Among Us Server Implementation"",
                        gameState_Lobby: ""Lobby"",
                        gameState_Started: ""In Game""
                    }},
                    zh: {{
                        title: ""Impostor 服务器状态"",
                        headerTitle: ""Impostor 服务器控制台"",
                        headerSubtitle: ""Impostor 状态监控与管理面板"",
                        serverVersion: ""服务器版本"",
                        activeRooms: ""活跃房间数"",
                        onlinePlayers: ""在线玩家"",
                        serverAddress: ""服务器地址"",
                        roomList: ""房间列表"",
                        roomCode: ""房间代码"",
                        status: ""状态"",
                        players: ""玩家"",
                        map: ""地图"",
                        impostors: ""内鬼"",
                        host: ""房主"",
                        noRoomsTitle: ""没有活跃的房间"",
                        noRoomsMessage: ""当前服务器上没有正在运行的游戏房间"",
                        footer: ""Impostor © {DateTime.Now.Year} - 基于 Among Us 的非官方服务器实现"",
                        gameState_Lobby: ""大厅中"",
                        gameState_Started: ""游戏中""
                    }}
                }};
                
                function initLanguage() {{
                    const savedLang = localStorage.getItem('language') || 'en';
                    applyLanguage(savedLang);
                    return savedLang;
                }}
                
                function applyLanguage(lang) {{
                    const elements = document.querySelectorAll('[data-i18n]');
                    elements.forEach(el => {{
                        const key = el.getAttribute('data-i18n');
                        if (i18n[lang] && i18n[lang][key]) {{
                            if (key === 'footer') {{
                                const year = new Date().getFullYear();
                                el.textContent = i18n[lang][key].replace('{DateTime.Now.Year}', year);
                            }} else {{
                                el.textContent = i18n[lang][key];
                            }}
                        }}
                    }});
                    
                    document.querySelectorAll('.game-status').forEach(el => {{
                        const state = el.getAttribute('data-state');
                        if (i18n[lang][`gameState_${{state}}`]) {{
                            el.textContent = i18n[lang][`gameState_${{state}}`];
                        }}
                    }});
                }}
                
                function toggleLanguage() {{
                    const currentLang = localStorage.getItem('language') || 'en';
                    const newLang = currentLang === 'en' ? 'zh' : 'en';
                    
                    localStorage.setItem('language', newLang);
                    applyLanguage(newLang);
                }}
                
                function initThemeToggle() {{
                    const themeToggle = document.getElementById('themeToggle');
                    const themeIcon = themeToggle.querySelector('i');
                    
                    const savedTheme = localStorage.getItem('theme') || 
                                      (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
                    document.body.setAttribute('data-theme', savedTheme);
                    updateThemeIcon(savedTheme);
                    
                    themeToggle.addEventListener('click', () => {{
                        const currentTheme = document.body.getAttribute('data-theme') || 'light';
                        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
                        
                        document.body.setAttribute('data-theme', newTheme);
                        localStorage.setItem('theme', newTheme);
                        updateThemeIcon(newTheme);
                    }});
                    
                    function updateThemeIcon(theme) {{
                        themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
                    }}
                }}
                
                document.addEventListener('DOMContentLoaded', () => {{
                    initThemeToggle();
                    
                    initLanguage();
                    
                    document.getElementById('languageToggle').addEventListener('click', toggleLanguage);
                }});
            </script>
        </body>
        </html>";

        return Content(html, "text/html");
    }

    private string GameToHtmlRow(IGame game)
    {
        var state = game.GameState.ToString();
        var statusClass = game.GameState == GameStates.Started ? "status-ingame" : "status-lobby";
        var hostName = game.Host?.Client.Name ?? "Unknown";

        string mapIcon = game.Options.Map switch
        {
            MapTypes.Skeld => "<i class=\"fas fa-space-shuttle\"></i>",
            MapTypes.MiraHQ => "<i class=\"fas fa-building\"></i>",
            MapTypes.Polus => "<i class=\"fas fa-mountain\"></i>",
            MapTypes.Airship => "<i class=\"fas fa-plane\"></i>",
            _ => "<i class=\"fas fa-map\"></i>"
        };

        return $@"
        <tr>
            <td><strong>{game.Code}</strong></td>
            <td>
                <span class=""status {statusClass} game-status"" data-state=""{state}"">
                    {(game.GameState == GameStates.Started ? "In Game" : "Lobby")}
                </span>
            </td>
            <td>{game.PlayerCount}/{game.Options.MaxPlayers}</td>
            <td><span class=""map-icon"">{mapIcon} {game.Options.Map}</span></td>
            <td>{game.Options.NumImpostors}</td>
            <td>{hostName}</td>
        </tr>";
    }
}
