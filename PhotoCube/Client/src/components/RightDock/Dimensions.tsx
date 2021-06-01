import React, { Component } from 'react';
import '../../css/RightDock/Dimensions.css';
import { Filter } from '../Filter';
import Dimension from './Dimension';
import PickedDimension from './PickedDimension';

/**
 * Container for Dimension.
 * Used in RightDock.
 */
export default class Dimensions extends Component<{
    onDimensionChanged:(dimName: string, dimension:PickedDimension) => void,
    onClearAxis:(axisName: string) => void,
    className: string,
    activeFilters: Filter[]
    }>{

    render(){
        return(
            <div className={this.props.className}>
                <h4 className="Header">Dimensions</h4>
                <div className="Container">
                    <Dimension xyz="X" activeFilters={this.props.activeFilters} onDimensionChanged={this.props.onDimensionChanged} onClearAxis={this.props.onClearAxis}/>
                    <Dimension xyz="Y" activeFilters={this.props.activeFilters} onDimensionChanged={this.props.onDimensionChanged} onClearAxis={this.props.onClearAxis}/>
                    <Dimension xyz="Z" activeFilters={this.props.activeFilters} onDimensionChanged={this.props.onDimensionChanged} onClearAxis={this.props.onClearAxis}/>
                </div>
            </div>
        );
    }
}