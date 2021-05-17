import React, { Component } from 'react';
import '../../css/Dimensions.css'
import { Filter } from '../Filter';
import PickedDimension from './PickedDimension';

export const FilterDropdown = 
    (props: {activeFilters: Filter[], onDimensionPicked: (dimension:PickedDimension) => void}) => {

    const createDimension = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const filter: Filter = JSON.parse(e.currentTarget.value);
        const dimension = ({
            id: filter.Id,
            name: filter.name,
            type: filter.type
        }) as PickedDimension;
        props.onDimensionPicked(dimension);
    }
    
    return (
        <select className="Filter Selector" onChange={(e) => createDimension(e)}>
            <option value="" selected disabled hidden>Select filter</option>
            {props.activeFilters.map(af => 
                <option value={JSON.stringify(af)}>{af.name}</option>)}
        </select>
    )
}
/**
 * Component repressenting a Dimension, can be either X, Y or Z based on this.props.xyz.
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
                    <FilterDropdown activeFilters={this.props.activeFilters} onDimensionPicked={this.dimensionPicked}/>
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