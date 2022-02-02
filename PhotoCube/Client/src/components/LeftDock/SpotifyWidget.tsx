import React from "react";
/**
 * 
 */
export const SpotifyWidget : any = (props:{spotifyURI:String | null}) => {
        return (
            <div className="spotifywidget">
                <iframe title="spotify" src={"https://open.spotify.com/embed/track/"+props.spotifyURI} width="300" height="80" frameBorder="0" allowTransparency={true} allow="encrypted-media"></iframe>
            </div>
        );
    
} 
