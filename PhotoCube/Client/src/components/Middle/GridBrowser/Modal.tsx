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
  toggleModal: () => void
  tags: string[]
}

const Modal: React.FC<FuncProps> = (props: FuncProps) => {
  return (
    <div>
      {props.show ? (
        <div className="modalContainer" onClick={() => props.toggleModal()}>
          <div className="modal">{props.tags.map((t) => (
              <h3>{t}</h3>
          ))}</div>
        </div>
      ) : null}
    </div>
  );
};

export default Modal;
