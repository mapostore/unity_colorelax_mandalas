-------------------------------------------------------------------------
--T and G Apps Ltd.
--www.tandgapps.co.uk
--support@tandgapps.co.uk

--CoronaSDK version 2014.2189a was used for this template.

--You are not allowed to publish this template to the Appstore as it is. 
--You need to work on it, improve it and replace the graphics. 

--For questions and/or bugs found, please contact me using our contact
--form on http://www.tandgapps.co.uk/contact-us/
-------------------------------------------------------------------------

--Localise
local M = {}
local json = require("json")
local url = require("socket.url")
local facebook 
local platform = "iOS" 
if system.getInfo("platformName") == "Android" then 
	platform = "Android"
	facebook = require("facebook") 
end 


-- Replace this value in the menus.lua file.
-- Its done there so that its in the same place this file is required.
M.facebookId = ""



--Facebook share function....
local function facebookShare( message )
	--Make our message.
	local message = message 
	
	--Old way, used for Android and ios5 devices.
	local function oldFacebook()
		--Print table test..
		--This can print the decoded array from facebook quite nicely.
		local tableName = {}; 
		local function printTable( t, level )
			level = level or 1
			if t then
				for k,v in pairs( t ) do
					local prefix = ""
					for i=1,level do prefix = prefix .. "\t" end
					tableName[#tableName+1] = k; --print( prefix .. "[" .. tostring(k) .. "] = " .. tostring(v) )
					if type( v ) == "table" then printTable( v, nil, level + 1 ) end
				end
			end
		end

		--Facebook Listener for posting...
		local function fbListener( event )
			--print_r(event)
			if event.isError then
				native.showAlert( "Share Score", "We're sorry, but something went wrong... please try again soon.", { "OK" } )
				return
			else
				if event.type == "session" then
					if event.phase ~= "login" then
						return
					else
						facebook.request( "me/feed", "POST", {message = message} ) 
					end
				elseif event.type == "request" then
					local response = json.decode( event.response )
					printTable(response)
					--print("*** TableName[1] is: ", tableName[1])

					if tableName[1] ~= "error" then
						native.showAlert( "Share Score", "You successfully posted your score on Facebook!", { "OK" } )
					end
				end
			end
		end     
		print("FACEOOK LOGIN: APPID:", M.facebookId)
		facebook.login( M.facebookId, fbListener, {"publish_actions"})
	end

	--Run platform checks
	if platform == "iOS" or platform == "simulator" then 
		--Run some extra checks to see if its newer than 5.0
		local isFacebookAvailable = native.canShowPopup( "social", "facebook" )
		if isFacebookAvailable ~= nil and isFacebookAvailable == true then --SOCIAL PLUGIN
 			local function socialEventListener(event) end --print_r(event) 
 			local options = {
			   service = "facebook",
			   message = message,
			   listener = socialEventListener,
			}
			native.showPopup( "social", options )			
		else
			-- oldFacebook()
			local alert = native.showAlert("Alert", "We couldn't share on Facebook, please make sure you are signed into Facebook in your phones 'Settings' menu", {"OK"})
		end

	--Android doesn't have the social plugin.
	elseif platform == "Android" and facebook ~= nil then 
		oldFacebook()
	end
end
M.facebookShare = facebookShare


--Twitter intent or popup share!
local function twitterShare( message )
	--Make our message.
	local message = message

	--Old systemOpenURL funct
	local function oldTwitter()
		local postBody = url.escape( message );
		system.openURL("http://twitter.com/intent/tweet?text="..postBody)
	end

	--Run platform checks
	if platform == "iOS" then 
		--Run some extra checks to see if its newer than 5.0
		local iosType = system.getInfo( "platformVersion" ) 
		local iosNum = string.sub(iosType, 1, 1)
		local isTwitterAvailable = native.canShowPopup( "social", "twitter" )	

		--Check iosNum
		if tonumber(iosNum) >= 6 and isTwitterAvailable ~= nil and isTwitterAvailable == true then --SOCIAL PLUGIN
 			local function socialEventListener(event)  end --print_r(event)
 			local options = {
			   service = "twitter",
			   message = message,
			   listener = socialEventListener,
			}
			native.showPopup( "social", options )

		elseif tonumber(iosNum) >= 5 then --OLDER POPUP
			local function twitterCallback(event) end
			local options = { message = message, listener = twitterCallback }
			native.showPopup("twitter", options)

		else
			oldTwitter()
		end

	--See if android can.
	else
		local isTwitterAvailable = native.canShowPopup( "social", "twitter" )	
		if isTwitterAvailable ~= nil and isTwitterAvailable == true then 
 			local function socialEventListener(event)  end --print_r(event)
 			local options = {
			   service = "twitter",
			   message = message,
			   listener = socialEventListener,
			}
			native.showPopup( "social", options )
		else
			oldTwitter()
		end
	end
end
M.twitterShare = twitterShare



--Return it now
return M