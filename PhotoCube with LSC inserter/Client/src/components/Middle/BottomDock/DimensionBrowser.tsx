import React, { useEffect, useState } from 'react';
import '../../../css/DimensionBrowser.css';
import Fetcher from '../ThreeBrowser/Fetcher';
import Dropdown from 'react-dropdown';
import 'react-dropdown/style.css';
import Tagset from '../ThreeBrowser/Tagset';
import { Filter } from '../../Filter';

const TagsetDropdown = (props: {onFilterAdded: (filters : Filter) => void}) => {

    const [options, setDropdownOptions] = useState([]);
    const [selected, setChoice] = useState({Id: 0, Name: ""});

    useEffect(() =>  {
        fetchTagsets();
        }
    , [])

    async function fetchTagsets () {
        const response = await Fetcher.FetchTagsets();
        const parsedResponse = response.map((ts : Tagset) => {return {"value" : ts.Id, "label" : ts.Name }})
        setDropdownOptions(parsedResponse);
    }

    const createFilter = () => {
        const filter : Filter = {
            Id: selected.Id,
            type: "tagset",
            name: selected.Name
        }
        return filter;
    }

    return (
        <div className="tagset dropdown">
            <Dropdown options={options} placeholder="Select a tagset" onChange={e => setChoice({Id: parseInt(e.value), Name: e.label!.toString()})}/>
            <button onClick={() => props.onFilterAdded(createFilter())}>Add Filter</button>
        </div>
    )
}

export const DimensionBrowser = (props: {onFilterAdded: (filters : Filter) => void}) => {
    return (
        <div id="dimension-browser">
            <TagsetDropdown onFilterAdded={props.onFilterAdded}/>
        </div>
    )
}
