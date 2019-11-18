What's this then?

I wanted a way to get my Logitech Media Server playlists into Home Assistant so I could pick from a dropdown and have it play.

The only real way I could do this with a potentially large payload was to write an external utility to grab the playlists from LMS, parse them, and call input_select.update_options in Home Assistent.

That's what this is.
