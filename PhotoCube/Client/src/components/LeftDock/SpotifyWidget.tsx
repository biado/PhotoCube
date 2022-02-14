import React from "react";
/**
 * component for playing music.
 */
export const SpotifyWidget : any = (props:{spotifyURI:String | null}) => {
        return (
            <div className="spotify_widget_component">
                <h5>Click to select track</h5>
                <iframe title="spotify" src={"https://open.spotify.com/embed/track/"+props.spotifyURI} width="215" height="80" frameBorder="0" allowTransparency={true} allow="encrypted-media"></iframe>
            </div>
        );
} 
