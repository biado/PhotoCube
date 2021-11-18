import React, { useState, useEffect } from "react";
import "../../../css/Modal.css";
import CubeObject from "../CubeBrowser/CubeObject";
import { BrowsingModes } from "../../RightDock/BrowsingModeChanger";
import Fetcher from "../CubeBrowser/Fetcher";
import { Image } from "../../../interfaces/types";
import { Filter } from "../../Filter";
import { TagsetDropdown } from "../BottomDock/TagsetFilter";

interface FuncProps {
  show: boolean;
  toggleModal: () => void;
  opTimelineBrowser: () => void;
  tags: string[];
  imageId: number;
  fileUri: string
}

const submitImage = async (fileUri: string) => {
  try {
    console.log(fileUri)
    Fetcher.SubmitImage(fileUri).then((r) => {
    console.log(r);
    });
  } catch (error) {
    console.error(error);
  }
};

const Modal: React.FC<FuncProps> = (props: FuncProps) => {
  return (
    <div>
      {props.show ? (
        <div className="modalContainer" onClick={() => props.toggleModal()}>
          <div className="modal">
            {props.tags.map((t) => (
              <h3>{t}</h3>
            ))}
            <footer className="modal_footer">
              <button className="modal-close" onClick={() => props.opTimelineBrowser()}>Open timelinebrowser</button>
              <button className="submit" onClick={() => submitImage(props.fileUri)}>Submit</button>
            </footer>
          </div>
        </div>
      ) : null}
    </div>
  );
};

export default Modal;
