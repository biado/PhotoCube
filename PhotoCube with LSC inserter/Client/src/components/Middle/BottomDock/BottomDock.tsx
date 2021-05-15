import React, { useState } from 'react';
import '../../../css/BottomDock/BottomDock.css';
import { MdExpandLess, MdExpandMore } from 'react-icons/md';
import { VscBrowser } from 'react-icons/vsc';
import { DimensionBrowser } from './DimensionBrowser';
import { Filter } from '../../Filter';

export const BottomDock = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {
    const [isExpanded, expand] = useState(false);

    return(
        <div className={isExpanded ? "bottom dock expanded" : "bottom dock"} >
            <div className="dimensionbrowser header">
                <div className="dock name">
                    <VscBrowser id="browser-icon" />
                    <h5>Dimension Browser</h5>
                </div>
                {isExpanded ? <MdExpandMore className="expand" onClick={e => expand(false)}/> : <MdExpandLess className="expand" onClick={e => expand(true)}/>}
            </div>
            {isExpanded ? <DimensionBrowser activeFilters={props.activeFilters} onFiltersChanged={props.onFiltersChanged}/> : null}
        </div>
    )
};