import React, { useEffect, useState } from 'react';
import { Filter } from '../../Filter';
import Fetcher from '../ThreeBrowser/Fetcher';
import Tagset from '../ThreeBrowser/Tagset';
import Dropdown, { Option } from 'react-dropdown';
import 'react-dropdown/style.css';
import '../../../css/TagsetFilter.css';

export const TagsetDropdown = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [selectedTagset, updateSelection] = useState<Tagset | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(false);

    useEffect(() =>  {
        fetchTagsets(); 
    }, []);

    async function fetchTagsets () {
        const response = await Fetcher.FetchTagsets();
        const tagsets = response.map((ts: Tagset) => {return {Id: ts.Id, Name: ts.Name }});
        setDropdownOptions(tagsets.map((ts: Tagset) => {return {value: ts.Id.toString(), label: ts.Name}}));
    }

    const addFilter = () => {
        const filter: Filter = {
            Id: selectedTagset!.Id,
            type: "tagset",
            name: selectedTagset!.Name 
        }
        if (!props.activeFilters.some(af => af.name === filter.name)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    const updateDropdown = (e: Option) => {
        updateSelection({Id: parseInt(e.value), Name: e.label!.toString(), Tags: null});
        disableButton(props.activeFilters.some(af => af.name == e.label!.toString()));
    }

    return (
        <div className="filter">
            <Dropdown options={options} placeholder="Select a tagset" onChange={e => updateDropdown(e)}/>
            <button className="add button" disabled={buttonDisabled} onClick={() => addFilter()}>Add Filter</button>
        </div>
    )
}