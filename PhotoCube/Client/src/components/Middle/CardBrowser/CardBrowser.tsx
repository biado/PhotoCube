import React, { Component, SyntheticEvent } from "react";
import CubeObject from "../CubeBrowser/CubeObject";
import Fetcher from "../CubeBrowser/Fetcher";
import "../../../css/CardBrowser.css";
import { BrowsingModes } from "../../RightDock/BrowsingModeChanger";
import Tag from "../CubeBrowser/Tag";
import { env } from "process";

/**
 * The CardBrowser allows the user to browse each photo one by one.
 */
export default class CardBrowser extends React.Component<{
  cubeObjects: CubeObject[];
  onBrowsingModeChanged: (browsingMode: BrowsingModes) => void;
  onSelectTrack: (spotifyURI: String) => void;
}> {
  state = {
    photoIndex: 0,
    currentPhotoClassName: "",
    spinnerVisibility: "hidden",
    photoVisibility: "visible",
    tagNamesWithCubeObjectId: [],
    imagesInCell: [],
  };

  render() {
    console.log(this.state.imagesInCell.length)
    if (this.state.imagesInCell.length > 0) {
      
      let fileName: string = "";
      if (this.state.imagesInCell[this.state.photoIndex]["fileURI"]) {
        fileName = this.state.imagesInCell[this.state.photoIndex]["fileURI"]!;
      }
      let thumbnail: string = "";
      if (this.state.imagesInCell[this.state.photoIndex]["thumbnailURI"]) {
        thumbnail = this.state.imagesInCell[this.state.photoIndex]["thumbnailURI"]!;
      }
      // if (thumbnail.includes(".jpg")){ //color
      //   thumbnail = "http://bjth.itu.dk:5002/images/colors/"+thumbnail
      // }
      return (
        <div className="grid-item cardBrowserContainer">
          <div>
            <p>
              {"Showing track: " +
                (this.state.photoIndex + 1) +
                " out of " +
                this.state.imagesInCell.length}
            </p>
            <br />
            <p>Spotify URI: {fileName}</p>
            <br />
            <div className="taglist container">
              <p>Tags:</p>
              <ul className="taglist cardmode">
                {this.state.tagNamesWithCubeObjectId.map((tag) => (
                  <li>{tag}</li>
                ))}
              </ul>
            </div>
          </div>
          <div className="currentPhotoContainer">
            <img
              id="currentPhoto"
              className={
                this.state.currentPhotoClassName +
                " " +
                this.state.photoVisibility
              }
              onLoad={(e) => this.onImageLoad(e)}
              //src={thumbnail}
              src={thumbnail.includes("/") ? thumbnail : "http://bjth.itu.dk:5002/images/colors/" + thumbnail}
              onClick={() => this.props.onSelectTrack(fileName)} //play in sp_widget
              //onClick={() => this.selectTrack(fileName)}
            ></img>
          </div>
        </div>
      );
    } else {
      return (
        <div className="grid-item cardBrowserContainer currentPhotoContainer">
          <p>Please choose some photos first.</p>
        </div>
      );
    }
  }

  /**
   * Get's tags associated with each and updates state.
   */
  /*   private async updateTagsInState() {
        if(this.props.cubeObjects.length > 0){
            await Fetcher.FetchTagsWithCubeObjectId(this.props.cubeObjects[this.state.photoIndex].Id)
            .then((tags:string[]) => {
                this.setState({tagNamesWithCubeObjectId: tags})
            });
        }   
        console.log(this.props.cubeObjects[this.state.photoIndex].Id);
    } */

  private async updateTagsInState() {
    if (this.state.imagesInCell.length > 0) {
      await Fetcher.FetchTagsWithCubeObjectId(
        this.state.imagesInCell[this.state.photoIndex]["id"]
      ).then((tags: string[]) => {
        this.setState({ tagNamesWithCubeObjectId: tags });
      });
    }
  }

  private async fetchAllImages() {
    await Fetcher.FetchAllImages().then((images: Object[]) => {
      this.setState({ imagesInCell: images });
    });
  }

  private async updateTagsAndFectImages() {
    try {
      await this.fetchAllImages();
      await this.updateTagsInState();
    } catch (error) {
      console.log(error);
    }
  }

  componentDidMount() {
    document.addEventListener("keydown", (e) => this.onKeydown(e));
    
    //document.addEventListener("onClick", (e) => this.onMouseClick(this.props.onSelectTack(this.state.imagesInCell[this.state.photoIndex]["fileURI"])))
    //document.addEventListener("onClick", (e) => this.onMouseClick(e))

    this.updateTagsAndFectImages();
  }

  componentWillUnmount() {
    document.removeEventListener("keydown", (e) => this.onKeydown(e));
  }

  /**
   * Showing spinner, however images are loaded too fast for the spinner to show.
   * @param e
   */
  onImageLoad(e: SyntheticEvent<HTMLImageElement, Event>) {
    this.setState({ spinnerVisibility: "hidden" });
    if (e.currentTarget.naturalWidth > e.currentTarget.naturalHeight) {
      this.setState({ currentPhotoClassName: "landscape" });
    } else {
      this.setState({ currentPhotoClassName: "portrait" });
    }
    this.setState({ photoVisibility: "visible" });
  }

  onLoadStart(e: SyntheticEvent<HTMLImageElement, Event>) {
    this.setState({ photoVisibility: "hidden" });
    this.setState({ spinnerVisibility: "visible" });
  }

  /**
   * Left arrow, Right arrow and Escape controls.
   * @param e
   */
  onKeydown(e: KeyboardEvent) {
    //console.log(e.key);
    if (e.key === "ArrowRight") {
      if (this.state.photoIndex < this.state.imagesInCell.length - 1) {
        this.setState({ photoIndex: this.state.photoIndex + 1 });
        this.updateTagsInState();
      }
    } else if (e.key === "ArrowLeft") {
      if (this.state.photoIndex != 0) {
        this.setState({ photoIndex: this.state.photoIndex - 1 });
        this.updateTagsInState();
      }
    } else if (e.key === "Escape") {
      this.props.onBrowsingModeChanged(BrowsingModes.Cube);
    }
  }

  private selectTrack = (filename: String) => {
    //console.log(filename); 
    this.props.onSelectTrack(filename)
  }

  private onMouseClick = (e: MouseEvent) => {
    if (e.button === 0 || e.button === 1) {
        if (this.state.imagesInCell.length > 0) {
            console.log(this.state.imagesInCell[this.state.photoIndex]["fileURI"])
            let spotifyUri = this.state.imagesInCell[this.state.photoIndex]["fileURI"]
            this.props.onSelectTrack(spotifyUri)
            //window.open("https://open.spotify.com/track/"+spotifyUri)
        } 
    }
  }
}
