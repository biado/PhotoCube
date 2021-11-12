import Axis from "./Axis";
import CubeObject from "./CubeObject";
import Tag from "./Tag";
import { Filter } from "../../Filter";

/**
 * The static Fetcher class is used to fetch data from the server.
 * Method calls are reused, and if the server address changes, we only need the change the baseUrl.
 */
export default class Fetcher {
  static baseUrl = process.env.REACT_APP_BASE_URL;
  static latestQuery = "";
  /**
   * Fetches Cells from the PhotoCube Server. See CellController.cs in server implementation.
   * @param xAxis
   * @param yAxis
   * @param zAxis
   */
  static async FetchCellsFromAxis(
    xAxis: Axis | null,
    yAxis: Axis | null,
    zAxis: Axis | null,
    filters: Filter[]
  ) {
    //Fetch and add new cells:
    let xDefined: boolean = xAxis !== null;
    let yDefined: boolean = yAxis !== null;
    let zDefined: boolean = zAxis !== null;

    let queryString: string = this.baseUrl + "/cell/?";
    if (xDefined) {
      queryString += "xAxis=" + this.parseAxis(xAxis!);
    }
    if (yDefined) {
      queryString += "&yAxis=" + this.parseAxis(yAxis!);
    }
    if (zDefined) {
      queryString += "&zAxis=" + this.parseAxis(zAxis!);
    }
    if (filters.length > 0) {
      queryString += "&filters=" + this.parseFilters(filters!);
    }
    console.log("querystring:", queryString);
    this.latestQuery = queryString;
    try {
      const response = await fetch(queryString);
      const data = await response.json();
      console.log("cubedata", data);
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  static async FetchAllImagesWithProjection(filters: Filter[]) {
    let queryString: string = this.baseUrl + "/cell/?";
    queryString += "&filters=" + this.parseFilters(filters!) + "&all=[]";
    try {
      const response = await fetch(queryString);
      const data = await response.json();
      console.log("cubedata", data);
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  private static parseFilters(filters: Filter[]): string {
    let sorted: Filter[] = filters.sort((a, b) => (a.type > b.type ? 1 : -1));
    var result: any[] = [];
    var dowfilter: any = { type: "tag", ids: [] };
    var semfilter: any = { type: "tag", ids: [] };
    for (var filter of filters) {
      switch (filter.type) {
        case "tagset":
          result.push({ type: "tagset", ids: [filter.id] });
          break;
        case "node":
          result.push({ type: "node", ids: [filter.id] });
          break;
        case "day of week":
          dowfilter.ids.push(filter.id);
          break;
        case "tag":
          semfilter.ids.push(filter.id);
          break;
        case "time":
          let name: string = filter.name;
          let ranges = name.split("-");
          result.push({ type: "timerange", ids: [3], ranges: [ranges] });
          break;
        default:
          result.push({ type: "tag", ids: [filter.id] });
          break;
      }
    }
    if (dowfilter.ids.length > 0) {
      result.push(dowfilter);
    }
    if (semfilter.ids.length > 0) {
      result.push(semfilter);
    }
    return JSON.stringify(result);
  }

  /**
   * Helper method for parsing an Axis into an object that is smaller than the Axis.ts class.
   * Is parsed to ParsedAxis.cs on the server.
   * Is a consice way of specifing an axis in a query to the server.
   * @param axis
   */
  private static parseAxis(axis: Axis): string {
    return JSON.stringify({
      type: axis.AxisType,
      id: axis.Id,
    });
  }

  static async FetchAllImages() {
    try {
      const response = await fetch(Fetcher.latestQuery + "&all=[]");
      const data = await response.json();
      console.log("allimages", data);
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  

  static async SubmitImage(fileUri: string) {
    try {
       const response = await fetch(`https://vbs.itec.aau.at:9443/api/v1/submit?item=${fileUri.slice(11, -4)}&session=${process.env.REACT_APP_SESSIONID}`)
       const data = await response.json()
       return data
    } catch (error) {
      console.error(error)
    }
  }

  /**
   * Returns all hierarchies.
   */
  static async FetchHierarchies() {
    return await fetch(Fetcher.baseUrl + "/hierarchy").then((result) => {
      return result.json();
    });
  }

  /**
   * Returns a single Hierarchy with hierarchyId.
   * @param hierarchyId
   */
  static async FetchHierarchy(hierarchyId: number) {
    return await fetch(Fetcher.baseUrl + "/hierarchy/" + hierarchyId).then(
      (result) => {
        return result.json();
      }
    );
  }

  /**
   * Fetches Tags that start with user-defined search term.
   * @param searchterm
   */
  static async FetchTagByName(searchterm: string) {
    return await fetch(Fetcher.baseUrl + "/tag/name=" + searchterm).then(
      (result) => {
        return result.json();
      }
    );
  }

  /**
   * Fetches all Tags that is in a tagset which has the given name.
   * Currently only works for tags of numerical type.
   * @param tagsetName
   */

  static async FetchTagsByTagsetName(tagsetName: string) {
    try {
      const response = await fetch(
        Fetcher.baseUrl + "/tagset/name=" + tagsetName
      );
      const data = await response.json();
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  /**
   * Fetches a single Node with nodeId from the server.
   * @param nodeId
   */
  static async FetchNode(nodeId: number) {
    try {
      const response = await fetch(Fetcher.baseUrl + "/node/" + nodeId);
      const data = await response.json();
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  /**
   * Fetches Nodes and it's immediate parent that start with the user-defined
   * search term.
   * @param searchterm
   */
  static async FetchNodeByName(searchterm: string) {
    return await fetch(Fetcher.baseUrl + "/node/name=" + searchterm).then(
      (result) => {
        return result.json();
      }
    );
  }

  /**
   * Fetches a node's parentnode.
   * @param nodeId
   */
  static async FetchParentNode(nodeId: number) {
    return await fetch(Fetcher.baseUrl + "/node/" + nodeId + "/parent").then(
      (result) => {
        return result.json();
      }
    );
  }

  /**
   * Fetches a node's children.
   * @param nodeId
   */
  static async FetchChildNodes(nodeId: number) {
    try {
      const call = Fetcher.baseUrl + "/node/" + nodeId + "/children";
      const response = await fetch(call);
      const data = await response.json();
      return data;
    } catch (error) {
      console.error(error);
    }
    /*  return await fetch(Fetcher.baseUrl + "/node/" + nodeId + "/children")
                .then(result => {return result.json()}); */
  }

  /**
   * Returns all tagsets.
   */
  static async FetchTagsets() {
    try {
      const response = await fetch(Fetcher.baseUrl + "/tagset");
      const data = await response.json();
      //console.log("fetchtagset", data)
      return data;
    } catch (error) {
      console.error(error);
    }
    /* return await fetch(Fetcher.baseUrl + "/tagset")
            .then(result => {return result.json()}); */
  }

  /**
   * Returns a single tagset with the tagsetId.
   * @param tagsetId
   */
  static async FetchTagset(tagsetId: number) {
    return await fetch(Fetcher.baseUrl + "/tagset/" + tagsetId).then(
      (result) => {
        return result.json();
      }
    );
  }

  /**
   * Returns an uri to the photo with the cubeobjectId
   * @param photoId
   */
  //Not in use:
  static GetPhotoURI(CubeObjectId: number): string {
    return Fetcher.baseUrl + "/photo/" + CubeObjectId;
  }

  /**
   * Fetches the tags that a cube object with cubeObjectId is tagged with.
   * @param cubeObjectId
   */
  /*  static async FetchTagsWithCubeObjectId(cubeObjectId: number){
          return await fetch(Fetcher.baseUrl + "/tag?cubeObjectId=" + cubeObjectId)
              .then(result => {return result.json()});
      } */

  static async FetchTagsWithCubeObjectId(cubeObjectId: number) {
    try {
      const response = await fetch(
        Fetcher.baseUrl + "/tag?cubeObjectId=" + cubeObjectId
      );
      const data = await response.json();
      console.log("image tag data", data);
      return data;
    } catch (error) {
      console.error(error);
    }
  }

  /* THE REST OF THE FILE IS NOT IN USE, BUT IS KEPT FOR ILLUSTRATIVE PURPOSES: */
  //Not in use:
  static async imageResult(PhotoId: number) {
    // Using sessionStorage as cache:
    let cachedValue = sessionStorage.getItem("photo/" + PhotoId);
    let imageResult: File;
    if (cachedValue != null) {
      //Cache hit!
      console.log("Cache hit in Photo!");
      imageResult = JSON.parse(cachedValue);
    } else {
      imageResult = await fetch(this.baseUrl + "photo/" + PhotoId).then(
        (result) => {
          return result.json();
        }
      );
      // Cache data using sessionStorage:
      sessionStorage.setItem("photo/" + PhotoId, JSON.stringify(imageResult));
    }
    return imageResult;
  }

  //Not in use:
  static async FetchCubeObjectsWithTag(tag: Tag) {
    return await fetch(this.baseUrl + "cubeobject/fromTagId/" + tag.id)
      .then((result) => {
        return result.json();
      })
      .then((cubeObjectArr: CubeObject[]) => {
        return cubeObjectArr;
      });
  }

  //Not in use:
  static async FetchCubeObjectsWith2Tags(tag1: Tag, tag2: Tag) {
    return await fetch(
      this.baseUrl + "cubeobject/from2TagIds/" + tag1.id + "/" + tag2.id
    )
      .then((result) => {
        return result.json();
      })
      .then((cubeObjectArr: CubeObject[]) => {
        return cubeObjectArr;
      });
  }

  //Not in use:
  static async FetchCubeObjectsWith3Tags(tag1: Tag, tag2: Tag, tag3: Tag) {
    return await fetch(
      this.baseUrl +
        "cubeobject/from3TagIds/" +
        tag1.id +
        "/" +
        tag2.id +
        "/" +
        tag3.id
    )
      .then((result) => {
        return result.json();
      })
      .then((cubeObjectArr: CubeObject[]) => {
        return cubeObjectArr;
      });
  }

  //Not in use:
  //OTR = ObjectTagRelations
  //Filters instead of getting new.
  // static async FetchCubeObjectsWithTagsOTR(tag1: Tag, tag2: Tag|null, tag3: Tag|null){
  //     // Using sessionStorage as cache:
  //     let cachedValue = sessionStorage.getItem("cubeobject/fromTagIdWithOTR/" + tag1.Id);
  //     let cubeObjectArrResult: CubeObject[];
  //     if(cachedValue != null){ //Cache hit!
  //         console.log("Cache hit!")
  //         cubeObjectArrResult = JSON.parse(cachedValue);
  //     }else{  //No cache hit, get data from server:
  //         cubeObjectArrResult = await fetch(this.baseUrl + "cubeobject/fromTagIdWithOTR/" + tag1.Id)
  //             .then(result => {return result.json();})
  //             .then((cubeObjectArr: CubeObject[]) => {
  //                 return cubeObjectArr
  //             });
  //         // Cache data using sessionStorage:
  //         sessionStorage.setItem("cubeobject/fromTagIdWithOTR/" + tag1.Id, JSON.stringify(cubeObjectArrResult));
  //     }
  //     //Filter:
  //     if(tag2 != null){
  //         //Filters out CubeObjects not tagged with tag2:
  //         cubeObjectArrResult = cubeObjectArrResult
  //             .filter(co => co.ObjectTagRelations!.some(otr => otr.TagId === tag2.Id));
  //     }if(tag3 != null){
  //         //Filters out CubeObjects not tagged with tag3:
  //         cubeObjectArrResult = cubeObjectArrResult
  //             .filter(co => co.ObjectTagRelations!.some(otr => otr.TagId === tag3.Id));
  //     }
  //     return cubeObjectArrResult;
  // }
  // Commented out because change to CubeObject affected co => co.ObjectTagRelations!.some... part and thus not compiled.
}
