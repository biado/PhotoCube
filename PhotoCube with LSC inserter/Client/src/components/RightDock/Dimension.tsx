import React, { Component, useEffect } from 'react';
import { useState } from 'react';
import '../../css/RightDock/Dimensions.css'
import { Filter } from '../Filter';
import PickedDimension from './PickedDimension';

/**
 * Dropdown component associated with each one of the three dimensions.
 * One active filter can be selected and projected on an axis. 
 */
export const FilterDropdown = 
    (props: {activeFilters: Filter[], onDimensionPicked: (dimension:PickedDimension) => void,
    cleared: boolean}) => {
    const [options, updateOptions] = useState<Filter[]>([]);
    const [selected, updateSelection] = useState<string>("");

    useEffect(() => {
        updateOptions(props.activeFilters.slice().reverse());
        if (props.cleared) { updateSelection(""); }
    }, [props.activeFilters, props.cleared])

    const createDimension = (e: React.ChangeEvent<HTMLSelectElement>) => {
        updateSelection(e.currentTarget.value);
        const filter: Filter = JSON.parse(e.currentTarget.value);
        const dimension = ({
            id: filter.Id,
            name: filter.name,
            type: filter.type
        }) as PickedDimension;
        props.onDimensionPicked(dimension);
    }
    
    return (
        <select className="Filter Selector" value={selected} onChange={(e) => createDimension(e)}>
            <option key={0} value={"true"}>Select filter</option>
            {options.map(af => (af.type === "tagset" || af.type === "hierarchy") ?
                <option key={af.Id} value={JSON.stringify(af)}>{af.name}</option> : null)}
        </select>
    )
}
/**
 * Component representing a Dimension, can be either X, Y or Z based on this.props.xyz.
 * Used in RightDock to choose values for dimensions.
 */
class Dimension extends Component<{
    xyz: string,
    onDimensionChanged:(dimName: string, dimension:PickedDimension) => void,
    onClearAxis: (axisName:string) => void,
    activeFilters: Filter[]
    }>{

    state = {
        DimensionType: null,
        DimensionId: null,
        DimensionName: null,
    };
    
    render(){
        return(
            <div className="Dimension">
                <p>{this.props.xyz}-Axis:</p><br/>
                {this.renderDimensionTypeAndName()}
                <div className="Dimension Selector">
                    <FilterDropdown 
                        cleared={this.state.DimensionName == null} 
                        activeFilters={this.props.activeFilters} 
                        onDimensionPicked={this.dimensionPicked}/>
                    <button onClick={() => this.onClearAxis(this.props.xyz)}>
                        Clear
                    </button>
                </div>
            </div>
        );
    }

    renderDimensionTypeAndName(){
        if(this.state.DimensionType != null){
            return (<p>{this.state.DimensionName} ({this.state.DimensionType})</p>);
        }else{
            return (<p>Choose a dimension...</p>)
        }
    }

    dimensionPicked = (dimension:PickedDimension) => {
        this.setState({
            DimensionType:  dimension.type, 
            DimensionId:    dimension.id, 
            DimensionName:  dimension.name
        });
        this.props.onDimensionChanged(this.props.xyz, dimension);
    }

    onClearAxis = (dimName: string) => {
        this.setState({DimensionType: null, DimensionId: null, DimensionName: null});
        this.props.onClearAxis(dimName);
    }
}

export default Dimension;