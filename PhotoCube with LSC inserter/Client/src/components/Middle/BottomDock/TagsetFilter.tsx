import React, { useEffect, useState } from 'react';
import { Filter } from '../../Filter';
import Fetcher from '../CubeBrowser/Fetcher';
import Tagset from '../CubeBrowser/Tagset';
import Dropdown, { Option } from 'react-dropdown';
import 'react-dropdown/style.css';
import '../../../css/BottomDock/TagsetFilter.css';

/**
 * Component for browsing and adding tagset filters. 
 */
export const TagsetDropdown = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [selectedTagset, updateSelection] = useState<Tagset | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(true);

    useEffect(() =>  {
        fetchTagsets(); 
    }, []);

    async function fetchTagsets () {
        const response = await Fetcher.FetchTagsets();
        const tagsets = response.map((ts: Tagset) => {return {Id: ts.Id, Name: ts.Name }});
        setDropdownOptions(tagsets.map((ts: Tagset) => {return {value: ts.Id.toString(), label: ts.Name}}));
    }

    const addFilter = () => {
        const filter: Filter = createFilter(selectedTagset!.Name, selectedTagset!.Id, "tagset", "", "");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    const updateDropdown = (e: Option) => {
        updateSelection({Id: parseInt(e.value), Name: e.label!.toString(), Tags: null});
        disableButton(props.activeFilters.some(af => af.Id === parseInt(e.value)));
    }

    return (
        <div className="Filter">
            <Dropdown options={options} placeholder="Select a tagset" onChange={e => updateDropdown(e)}/>
            <button className="add button" disabled={buttonDisabled} onClick={() => addFilter()}>Add filter</button>
        </div>
    )
}

//utility function
export const createFilter = (tagName: string, id: number, type: string, startTime: string, endTime: string) => {
    const filter: Filter = {
        Id: id,
        type: type,
        name: tagName,
        startTime: startTime,
        endTime: endTime
    }
    return filter;
}