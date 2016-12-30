using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projekt.Controllers
{

    public class WebSocketHandlerServer
    {

        public static async Task ChatHandler(HttpContext http, Func<Task> next)
        {

            string email = "";
            bool if_recived = true;
            if (http.WebSockets.IsWebSocketRequest)
            {

                var webSocket = await http.WebSockets.AcceptWebSocketAsync();

                while (webSocket != null && webSocket.State == WebSocketState.Open)
                {

                    using (var context = new ApplicationDbContext())
                    {
                        List<Chat> messagescount = context.chat.Where(d => d.toperson == email).Where(d => d.sendedmsg == false).ToList();
                        for (int i = 0; i < messagescount.Count; i++)
                        {
                            var buffor2 = new ArraySegment<Byte>(Encoding.UTF8.GetBytes(messagescount[i].author + " :" + messagescount[i].message));
                            messagescount[i].sendedmsg = true;
                            context.chat.Update(messagescount[i]);
                            context.SaveChanges();

                            await webSocket.SendAsync(buffor2, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }

                    if (if_recived == true) // tworzy tylko jezeli otrzymal wiadomosc poprzednio wypluwał 1000 watkow na minute ;d
                    {
                        Console.WriteLine("nowy watek !!");
                        if_recived = false;
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(
                        async delegate (object o, DoWorkEventArgs args)
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                if (webSocket.State != WebSocketState.CloseReceived)
                                {
                                    var buffer = new ArraySegment<Byte>(new Byte[512]);
                                    var received = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                                    if_recived = true;
                                    if (received != null && webSocket.State != WebSocketState.CloseReceived)
                                    {

                                        var request = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                                        if (!request.Contains(@"0\0")) // closestring jest taki dziwny :C
                                        {
                                            string[] split = request.Split(new Char[] { '%' });

                                            email = split[0];

                                            var msg = new Chat()
                                            {
                                                author = split[0],
                                                message = split[1],
                                                messagedate = DateTime.Now,
                                                toperson = split[2],
                                                sendedmsg = false,
                                            };

                                            context.chat.Add(msg);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        });
                        bw.RunWorkerAsync();
                        bw.Dispose();
                    }
                }
            }
            else
            {
                await next();
            }
        }
    }
}
