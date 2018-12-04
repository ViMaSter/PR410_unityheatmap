using UnityEngine;

// supress 'warning CS0414: The private field `[BLANK]' is assigned but its value is never used' messages
//   as they are assigned/read when constructing/parsing JSON messages with JsonUtility.ToJson()/JsonUtility.FromJson<>()
#pragma warning disable 0414

namespace NetworkDefinitions
{
	[System.Serializable]
	public abstract class GameData
	{
	}
	[System.Serializable]
	public abstract class PlayerData
	{
	}

	namespace Request
	{
		[System.Serializable]
		public class JoinSession
		{
			[SerializeField]
		    private string command = "joinSession";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			// valid sessions 0-Int32.MaxValue; -1 = last available session
			[SerializeField]
		    private int sessionID = int.MinValue;
		    public int SessionID
		    {
		    	get
		    	{
		    		return sessionID;
		    	}
		    }

		    // without an explicit sessionID specified, join the last available session
		    public JoinSession(int sessionID = -1)
		    {
				this.sessionID = sessionID;
		    }
		}

		[System.Serializable]
		public class LeaveSession
		{
			[SerializeField]
		    private string command = "leaveSession";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

		    public LeaveSession()
		    {
		    }
		}

		[System.Serializable]
		public class CreateSession<CustomGameData, CustomPlayerData>
			where CustomGameData : NetworkDefinitions.GameData
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "createSession";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }
			[SerializeField]
		    private CustomGameData session;
		    public CustomGameData Session
		    {
		    	get
		    	{
		    		return session;
		    	}
		    }
			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    public CreateSession(CustomGameData session, CustomPlayerData player)
		    {
		    	this.session = session;
		    	this.player = player;
		    }
		}

		[System.Serializable]
		public class UpdateSession<CustomGameData, CustomPlayerData>
			where CustomGameData : NetworkDefinitions.GameData
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "updateSession";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }
			[SerializeField]
		    private CustomGameData session;
		    public CustomGameData Session
		    {
		    	get
		    	{
		    		return session;
		    	}
		    }
			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    public UpdateSession(CustomGameData session, CustomPlayerData player)
		    {
		    	this.session = session;
		    	this.player = player;
		    }
		}

		[System.Serializable]
		public class UpdatePlayer<CustomPlayerData>
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "updatePlayer";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }
			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    public UpdatePlayer(CustomPlayerData player)
		    {
		    	this.player = player;
		    }
		}
	}

	namespace Response
	{
		[System.Serializable]
		public class SessionJoin<CustomGameData, CustomPlayerData>
			where CustomGameData : NetworkDefinitions.GameData
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			[SerializeField]
		    private int error = -1;
		    public int Error
		    {
		    	get
		    	{
		    		return error;
		    	}
		    }

			[SerializeField]
		    private int sessionID;
		    public int SessionID
		    {
		    	get 
		    	{
		    		return sessionID;
		    	}
		    }
			[SerializeField]
		    private int playerID;
		    public int PlayerID
		    {
		    	get 
		    	{
		    		return playerID;
		    	}
		    }
			[SerializeField]
		    private CustomGameData session;
		    public CustomGameData Session
		    {
		    	get
		    	{
		    		return session;
		    	}
		    }
			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    private SessionJoin()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	if(Command != "sessionJoin") return false;
			    	if(SessionID == -1) return false;
			    	if(PlayerID == -1) return false;
			    	if(Error != 0) return false;
			    	return true;
		    	}
		    }
		}

		[System.Serializable]
		public class SessionUpdate<CustomGameData, CustomPlayerData>
			where CustomGameData : NetworkDefinitions.GameData
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			[SerializeField]
		    private CustomGameData session;
		    public CustomGameData Session
		    {
		    	get
		    	{
		    		return session;
		    	}
		    }

			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    private SessionUpdate()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	if(Command != "sessionUpdate") return false;
			    	return true;
		    	}
		    }
		}

		[System.Serializable]
		public class PlayerJoin<CustomPlayerData>
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			[SerializeField]
		    private int error = -1;
		    public int Error
		    {
		    	get
		    	{
		    		return error;
		    	}
		    }
			[SerializeField]
		    private int playerID;
		    public int PlayerID
		    {
		    	get 
		    	{
		    		return playerID;
		    	}
		    }

			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    private PlayerJoin()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	if(Command != "playerJoin") return false;
			    	if(Error != 0) return false;
			    	return true;
		    	}
		    }
		}

		[System.Serializable]
		public class PlayerUpdate<CustomPlayerData>
			where CustomPlayerData : NetworkDefinitions.PlayerData
		{
			[SerializeField]
		    private string command = "";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			[SerializeField]
		    private int error = -1;
		    public int Error
		    {
		    	get
		    	{
		    		return error;
		    	}
		    }
			[SerializeField]
		    private int playerID;
		    public int PlayerID
		    {
		    	get 
		    	{
		    		return playerID;
		    	}
		    }

			[SerializeField]
		    private CustomPlayerData player;
		    public CustomPlayerData Player
		    {
		    	get
		    	{
		    		return player;
		    	}
		    }

		    private PlayerUpdate()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	if(Command != "playerUpdate") return false;
			    	if(Error != 0) return false;
			    	return true;
		    	}
		    }
		}

		[System.Serializable]
		public class SessionLeave
		{
			[SerializeField]
		    private string command = "";
		    public string Command
		    {
		    	get
		    	{
					if (command == null) return "";
		    		return command;
		    	}
		    }

			[SerializeField]
		    private int error = -1;
		    public int Error
		    {
		    	get
		    	{
		    		return error;
		    	}
		    }

		    private SessionLeave()
		    {
		    }

		    public bool IsValid
		    {
		    	get
		    	{
			    	if(Command != "sessionLeave") return false;
			    	if(Error != 0) return false;
			    	return true;
		    	}
		    }
		}
	}
}

namespace Game
{
	[System.Serializable]
	public class SessionData : NetworkDefinitions.GameData
	{
		[SerializeField]
		private string mapName = "";
		public string MapName
		{
			get
			{
				if (mapName == null) return "";
				return mapName;
			}
		}
		
		[SerializeField]
		private long timelimit = -1;
		public long Timelimit
		{
			get
			{
				return timelimit;
			}
		}
		
		[SerializeField]
		private long currentMatchStart = -1;
		public long CurrentMatchStart
		{
			get
			{
				return currentMatchStart;
			}
		}

	    public SessionData(string mapName, long timelimit, long currentMatchStart)
	    {
			this.mapName = mapName;
			this.timelimit = timelimit;
			this.currentMatchStart = currentMatchStart;
	    }
	}

	[System.Serializable]
	public class PlayerData : NetworkDefinitions.PlayerData
	{
		[SerializeField]
		private string name = "";
		public string Name
		{
			get
			{
				if (name == null) return "";
				return name;
			}
		}

		[SerializeField]
		private Vector2 position = new Vector2(-1, -1);
		public Vector2 Position
		{
			get
			{
				return position;
			}
		}

		[SerializeField]
		private int colorHex = -1;
		public Color32 ColorHex
		{
			get
			{
				Color32 color =  new Color32();
				color.b = (byte)((colorHex) & 0xFF);
				color.g = (byte)((colorHex>>8) & 0xFF);
				color.r = (byte)((colorHex>>16) & 0xFF);
				color.a = 0xFF;
				return color;
			}
		}

	    public PlayerData(string name, Vector2 position, Color32 colorHex)
	    {
			this.name = name;
			this.position = position;
			this.colorHex = colorHex.r << 16 |
							colorHex.g << 8 |
							colorHex.b;
	    }
	}
}