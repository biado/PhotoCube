import React, { useEffect, useState } from 'react';
import '../../../css/BottomDock.css';
import { MdExpandLess, MdExpandMore } from 'react-icons/md';
import { VscBrowser } from 'react-icons/vsc';
import { DimensionBrowser } from './DimensionBrowser';
import { Filter } from '../../Filter';

export const BottomDock = (props: {onFiltersChanged: (filters : Filter[]) => void}) => {
    const [isExpanded, expand] = useState(false);
    const [filters, setFilters] = useState<Filter[] | []>([]);

    const addFilter = (filter : Filter) => {
        setFilters([...filters, filter]);
    }

    useEffect(() => {  
        if (filters.length !== 0) {
        props.onFiltersChanged(filters);
        }
    }, [filters])

    return(
        <div className={isExpanded ? "bottom dock expanded" : "bottom dock"} >
            <div className="dimensionbrowser header">
                <div className="dock name">
                    <VscBrowser id="browser-icon" />
                    <h5>Dimension Browser</h5>
                </div>
                {isExpanded ? <MdExpandMore className="expand" onClick={e => expand(false)}/> : <MdExpandLess className="expand" onClick={e => expand(true)}/>}
            </div>
            {isExpanded ? <DimensionBrowser onFilterAdded={addFilter}/> : null}
        </div>
    )
};