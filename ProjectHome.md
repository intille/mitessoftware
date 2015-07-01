This is the code repository for MIT's MITes Data Collection Software. The software consists of a set of libraries that collect data from multiple MITes receivers. The code is written in C# and is released under an MIT Public License.

The software is organized into 3 components:

**1- The Source Tree:** This includes the C# source files and are located under the "Source" tab. Two versions of the source code exist. The first version is under svn/trunk/stable and is the current stable version of the code. The second version is under svn/trunk/development and should not be assumed to be stable but includes the most recent updates to the code.

**2- The MITesProjects.zip:** The file includes the project files for visual studio for different platforms (currently PC and PocketPC).

**3- The Bin.zip:** The file includes some of the needed files to compile the project in visual studio and will include the binaries generated during compilation.


## Instructions for Compiling the Code and Running with Attached MITes Sensors ##

1- First, make sure you have the right number of MITes receivers connected to your machine.

2- Download the source tree. (Go to the source tab for instructions). Use the development version, go to svn/trunk/development.

3- Download the most recent zip file for Visual Studio projects  [Development MITes Projects](http://mitessoftware.googlecode.com/files/MITesProjects-v9.zip).

4- Download the latest bin zip file [Development Bin Zip File](http://mitessoftware.googlecode.com/files/bin-v9.zip).

5- Uncompress both zip files in the root source code directory (i.e. where `MITesSRC/` exists) so that you end up with the MITesSRC directory, the MITesProjects-v9 and the bin-v9 directories at the same level. For the development version, uncompress the zip files under `develeopment/`.

6- Inside the unizippped `MITesProjects-v9/` folder, you will find `MITesProjects/` folder. Move this folder to its parent directory `development/`. Similarly, move the `bin/` folder inside `bin-v9/` to the `development/` directory. You should have the following 3 folders in the `development/` directory: `MITesSRC/`, `bin/` and `MITesProjects/`.

7- To run the PC version, navigate under the source directory to `MITesProjects/PC` and click MITesDataCollection.sln. Alternatively, to run the Pocket PC version, navigate under the source directory to `MITesProjects/PPC` and click MITesDataCollection.sln. This will load the appropriate solution in Visual Studio.

8- Using Visual Studio's solution explorer, set the `MITesDataCollection` project as the startup project.


9- Rebuild the solution.

10- Choose the 'start debugging' option under Debug. The code should run.

For instructions on using the software, please visit [the following page](http://web.mit.edu/wockets/MITes/data/MITesDataCollectionGettingStarted.html).


## Instructions for Compiling the Code and Loading MITes Data Offline ##

1- Download the source tree. (Go to the source tab for instructions). For the stable version, go to svn/trunk/stable. For the development version, go to svn/trunk/development.

2- Download the most recent zip file for Visual Studio projects  [MITes Projects](http://mitessoftware.googlecode.com/files/MITesProjects-v9.zip).

3- Download the latest bin zip file [Bin Zip File](http://mitessoftware.googlecode.com/files/bin-v9.zip).

4- Uncompress both zip files in the root source code directory (i.e. where `MITesSRC/` exists) so that you end up with the MITesSRC directory, the MITesProjects-v9 and the bin-v9 directories at the same level. For the development version, uncompress the zip files under `develeopment/`. For the stable version, uncompress the zip files under `stable/`.

5- Inside the unizippped `MITesProjects-v9/` folder, you will find `MITesProjects/` folder. Move this folder to its parent directory `development/`. Similarly, move the `bin/` folder inside `bin-v9/` to the `development/` directory. You should have the following 3 folders in the `development/` directory: `MITesSRC/`, `bin/` and `MITesProjects/`.

6- Run the PC version, navigate under the source directory to `MITesProjects/PC` and click MITesDataCollection.sln. This will load the appropriate solution in Visual Studio.

8- Using Visual Studio's solution explorer, set the `TestApplication` project as the startup project. The TestApplication makes a call to 'toArff' that loads a sample data set in PLFormat from `bin\NeededFiles\SamplePLFormat` and calculates features vectors on the data and stores the resulting vectors in a [Weka ARFF File](http://www.cs.waikato.ac.nz/~ml/weka/arff.html).


9- Rebuild the solution.

10- Choose the 'start debugging' option under Debug. The code should run.

For instructions on using the software, please visit [the following page](http://web.mit.edu/wockets/MITes/data/MITesDataCollectionGettingStarted.html).