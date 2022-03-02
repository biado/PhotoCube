import { Console } from 'console';
import React from 'react';
import '../../css/RightDock/FileCount.css';
/**
 * FileCount is a Component that shows how many unique photos the CubeBrowser is currently showing.
 */
class FileCount extends React.Component<{
        className: string,
    }>{
    state = {
        fileCount: 0,
    };


    componentDidMount(){
        
    }
    handleClick = () =>{}

    render(){
        return(
            <div className={this.props.className} id="FileCount">
                {/* <h4 className="Header">File count</h4> */}
                <div className="Content">
                    <p>Showing: </p>
                    <p>{this.state.fileCount}</p>
                    <p>{this.state.fileCount==1? " cube" : " cubes"}</p>
                </div>
            </div>
        );
    }

    /**
     * Update file count from outside.
     * @param count The updated filecount.
     */
    UpdateFileCount(count: number){
        this.setState({fileCount: count});
    }
}

export default FileCount;

