function main()
{
	print("x:" + me.x + " y:" + me.y);
	this.checkCords = false
	addEventListener("keyup", function(key) {

		if(key == 0xBC)
		{
			this.checkCords = true;
		}
		if(key == 0xBE)
		{
			this.checkCords = false;
		}
	while(this.checkCords)
	{
		say("!X:" + me.x + " Y:" + me.y);
		delay(1000);
	}
		});

	while(1)
	{delay(1000);}
}