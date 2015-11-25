/**
*	@filename	clickHelper.js
*	@author		dzik
*	@encoding	ANSI
*	@version	2015.08.21 12:30
*	@info		Some functions are taken from kolton's item mover.
*					- http://pastebin.com/7bZ33xcc
*				Require latest kolbot with CORE15.
*					- https://github.com/kolton/d2bot-with-kolbot/archive/CORE15.zip
*				Please check wiki if you have problems with starting bot.
*					- https://github.com/kolton/d2bot-with-kolbot/wiki
*				Release topic.
*					- http://www.blizzhackers.cc/viewtopic.php?f=209&t=502639
*	@install	Save into kolbot/libs/bots.
*				Add to char config: Scripts.clickHelper = true;
*				Use D2BotBlank.dbj entry point.
*	@update		2015.08.25
*					Dupe is fixed. 
*					Don't use 0x44 to gate while in nihla interaction.
*					Don't use remove nihla while in trade.
*/

function clickHelper () {	
	var hooks = [], decor = [], activeAction, end, block, wpgid, resolution = me.screensize, resfix, result,trapp,lock = false,sendGame = false;
	
	addEventListener("gamepacketsent", function (pBytes) 
	{ 
		if (pBytes[0] == 0x30) 
		{ return block; } 
		else 
		{ return false; }
		if(pBytes[0] == 0x4f && pBytes[1]==0x02)
			{
				print("test");
				pBytes[2] = 0x03;
				return true;
			}
	});
	addEventListener("gamepacket", function (pBytes) 
	{ 

	});
        addEventListener("chatmsg",             
            function onMsg(nick, msg) {
                    if (msg)
                            wpgid = msg.split("A5WP ")[1];
            }
    );
	// Sort items by size to pick biggest first
	function sortPickList(a, b) {
		// Same size -  sort by distance
		if (b.sizex === a.sizex && b.sizey === a.sizey) { 
			getDistance(me, a) - getDistance(me, b);
			
		}
		
		return b.sizex * b.sizey - a.sizex * a.sizey;
		
	}
	
	// Sort items by size to move biggest first
	function sortMoveList(a, b) {		
		return b.sizex * b.sizey - a.sizex * a.sizey;
		
	}
	
	function hookHandler (click, x, y) {
		// Get the hook closest to the clicked location
		function sortHooks(h1, h2) {
			return Math.abs(h1.y - y) - Math.abs(h2.y - y);
			
		}
		
		// Left click
		if (click === 0) {
			// Sort hooks
			hooks.sort(sortHooks);
			
			// Don't start new action until the current one finishes
			if (activeAction && activeAction !== hooks[0].text) {
				return true;
				
			}
			
			// Toggle current action on/off
			activeAction = activeAction ? false : hooks[0].text;
			
			hooks[0].color = hooks[0].color === 2 ? 1 : 2;
			
			// Block click
			return true;
			
		}
		
		return false;
		
	}
	
	function showHooks () {
		/* me.screensize return
			0 for 640x480
			1 for 800x600
		*/
		
		resfix = me.screensize ? 0 : -120;
		
		// if we have hooks already check for resolution change		
		if (hooks.length) {
			if (resolution != me.screensize) {
				resolution = me.screensize;
				
				while (hooks.length) {
					var kill = hooks.shift();
					kill.remove();
					
				}
				
				while (decor.length) {
					var kill = decor.shift();
					kill.remove();
					
				}
				
			} else {
				return false;
				
			}
			
		}
	
		var commands = [
			"get a5", "give a5", "go to a5", 
			"move inv to trade",
 			 "sell from trade", "dupe","X: Y:",
			"Show/Hide"
		];
		
		for (var i = commands.length; i; i--) {
			addHook (commands[i-1]);
			
		}
		
		return true;
		
	}
	
	function addHook (text) {
		decor.push (new Frame (6, 466 - hooks.length * 19 + resfix, 160, 16));
		decor.push (new Box (6, 466 - hooks.length * 19 + resfix, 160, 16));
		hooks.push (new Text (text.toUpperCase(), 13, 480 - hooks.length * 19 + resfix, 2, 0, 0, false, text == "X: Y:" ? null:hookHandler));
		
	}
	
	while (true) {
		if(!sendGame)
		{
			sendGame = true;
		}
		switch (activeAction) {
			case "DUPE":

				// sendPacket(1, 0x44, 4, 2, 4, shrine.gid, 4, getUnit(100).gid, 4, 0x03);
				var tmp = getUnit(-1, "dummy");
				var bool = true;
				while(bool)
				{
					if(tmp.classid == 102)
					{
						print("test");
						tmp = tmp;
						bool = false;
					}
					else
					{
						tmp.getNext();
					}
				}
				getPacket(1, 0x0a, 1, tmp.type, 4, tmp.gid);
				activeAction = false;
			break;
			case "GET A5":
				if (!wpgid) {
                            print("we dont have wp GID. Can't access A5");
                            activeAction = false;
                            break;
                        }
                        
                        Town.move("portalspot");
                        
                        while (!Pather.usePortal (4, null)) {
                                delay (1);
                                
                        }
                        
                        // Waypoint UI
                        while (!getUIFlag (0x14)) {
                                Pather.moveTo (5116,5067,5);
                                sendPacket (1, 0x13, 4, 0x02, 4, wpgid);
                                delay (100);
                                
                        }
                        
                        while (getUIFlag (0x14)) {
                                me.cancel ();
                                
                        }
                        
                        Pather.usePortal (1, null);
                        Pather.useWaypoint (109);
                        activeAction = false;

				break;
				
			case "GIVE A5":
				 Pather.journeyTo(4);
                        
                try {
                        result = getCollision(4, 5114, 5069);
                        
                } catch (e) {
                        activeAction = false;
                        break;
                        
                }

                // Avoid non-walkable spots, objects
                if (result === undefined || (result & 0x1) || (result & 0x400)) {
                        activeAction = false;
                        break;
                        
                }
                
                Pather.moveTo(5114,5069,5);
                
                Pather.makePortal (true);
                
                Pather.useWaypoint (109);
                
                while (!me.area) delay (100);
                
                if (me.act === 5) {
                        say("A5WP " + getUnit(2, "Waypoint").gid);
                }                                               
                
                activeAction = false;
                
				break;	
				
			case "GO TO A5":
				Pather.useWaypoint (109);
			
				activeAction = false;
				break;

			case "MOVE INV TO TRADE":
				var inv = me.findItems (-1, 0, 3);
				inv.sort(sortMoveList);
								
				while (activeAction && inv.length) {
					var that = inv.shift();
					
					if (getUIFlag(0x17) && Storage.TradeScreen.CanFit(that)) {
						Storage.TradeScreen.MoveTo(that);
						
					}

				}				
			
				activeAction = false;
				break;
				
			case "SELL FROM TRADE":
				var unit = me.getItem();
				var npc = getInteractedNPC();
				
				if (npc) {
					if (unit) {
						do {
							if (unit.location == 4) {
								print(unit.name+" sold");
								sendPacket (1, 0x33, 4, npc.gid, 4, unit.gid, 4, unit.mode, 4, 0);
								delay (me.ping * 2 + 100);
							}
							
						} while (unit.getNext ());
						
					}
					
				}
			
				activeAction = false;
				break;
			case "SHOW/HIDE":
				for (var i = 0; i < hooks.length; i++) {
					if(hooks[i].visible == true && hooks[i].text != "SHOW/HIDE")
						hooks[i].visible = false;
					else if (hooks[i].visible == false && hooks[i].text != "SHOW/HIDE")
					{
						hooks[i].visible = true;
					}
				};

				for (var i = 0; i < decor.length; i++) {
					if(decor[i].visible == true)
						decor[i].visible = false;
					else
						decor[i].visible = true;
				};

				activeAction = false;
			break;

			default:
				showHooks ();
				
				for (var i = 0; i < hooks.length; i++) {
					if (hooks[i].color === 1) {
						hooks[i].color = 2;
						
					}
				}
				
				break;
		}

		for (var i = 0; i < hooks.length; i++) {
			if(hooks[i].text.search("X") == 0)
			{
				hooks[i].text = "X: " + me.x + " Y: " + me.y;
			}
			
		};
		
		delay (10);
		
	}
	return true;
	
}

// Storage override
Storage.Stash.MoveTo = function (item) {
	var i, spot, tick;

	if (Packet.itemToCursor(item)) {
		for (i = 0; i < 15; i += 1) {
			spot = Storage.Stash.FindSpot(item); // Returns inverted coords...

			if (spot) {
				// 18 [DWORD id] [DWORD xpos] [DWORD ypos] [DWORD buffer]
				sendPacket(1, 0x18, 4, item.gid, 4, spot.y, 4, spot.x, 4, 0x4);
			}

			tick = getTickCount();

			while (getTickCount() - tick < Math.max(1000, me.ping * 2 + 200)) {
				if (!me.itemoncursor) {
					return true;
				}

				delay(10);
			}
		}
	}

	return false;
};

Storage.Cube.MoveTo = function (item) {
	var i, spot, tick;

	if (Packet.itemToCursor(item)) {
		for (i = 0; i < 15; i += 1) {
			spot = Storage.Cube.FindSpot(item); // Returns inverted coords...

			if (spot) {
				// 18 [DWORD id] [DWORD xpos] [DWORD ypos] [DWORD buffer]
				sendPacket(1, 0x18, 4, item.gid, 4, spot.y, 4, spot.x, 4, 0x3);
			}

			tick = getTickCount();

			while (getTickCount() - tick < Math.max(1000, me.ping * 2 + 200)) {
				if (!me.itemoncursor) {
					return true;
				}

				delay(10);
			}
		}
	}

	return false;
};

Storage.TradeScreen.MoveTo = function (item) {
	var i, spot, tick;

	if (Packet.itemToCursor(item)) {
		for (i = 0; i < 15; i += 1) {
			spot = Storage.TradeScreen.FindSpot(item); // Returns inverted coords...

			if (spot) {
				// 18 [DWORD id] [DWORD xpos] [DWORD ypos] [DWORD buffer]
				sendPacket(1, 0x18, 4, item.gid, 4, spot.y, 4, spot.x, 4, 0x2);
			}

			tick = getTickCount();

			while (getTickCount() - tick < Math.max(1000, me.ping * 2 + 200)) {
				if (!me.itemoncursor) {
					return true;
				}

				delay(10);
			}
		}
	}

	return false;
};

