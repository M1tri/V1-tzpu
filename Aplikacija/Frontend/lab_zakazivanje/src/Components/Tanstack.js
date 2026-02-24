import React, { useState, useMemo } from "react";
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  getPaginationRowModel,
  getSortedRowModel,
  getFilteredRowModel
} from "@tanstack/react-table";
import 'bootstrap-icons/font/bootstrap-icons.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMagnifyingGlass, faPenToSquare, faGear, faCircleUp, faCircleDown, faEye, faClone, faTrashCan } from '@fortawesome/free-solid-svg-icons';
import { parseDate, parseTime } from "../Utils/helper";
import { SessionView } from "../DTOs/session";

export default function Tanstack({tableData, naslov, enableFeatures = true, 
                                newlyAddedId, setNewlyAddedId, onEditClicked, 
                                onSetNext, onSetPlanned, onSetActive, onViewResources,
                                onSessionCloned, onSessionDeleted})
{
    const [pageIndex, setPageIndex] = useState(0);
    const [pageSize, setPageSize] = useState(3);
    const [sorting, setSorting] = useState([]);
    const [globalFilter, setGlobalFilter] = useState("");

    const columns = useMemo(() => {

        const baseCols = [
        { 
            header: ({ column }) => (
                <div>
                Naziv aktivnosti
                {enableFeatures && column.getIsSorted() === "asc" && <i className="bi bi-sort-alpha-down ms-2"></i>}
                {enableFeatures && column.getIsSorted() === "desc" && <i class="bi bi-sort-alpha-down-alt"></i>}
                {enableFeatures && !column.getIsSorted() && column.getCanSort() && <i className="bi bi-arrow-down-up ms-2"></i>}
                </div>
            ),
            accessorKey: "naziv", 
            enableSorting: true 
        },

        { 
            header: ({ column }) => (
                <div style={{ display: "flex", alignItems: "center", gap:"5px" }}>
                Tip aktivnosti
                {enableFeatures && column.getIsSorted() === "asc" && <i className="bi bi-sort-alpha-down ms-2"></i>}
                {enableFeatures && column.getIsSorted() === "desc" && <i class="bi bi-sort-alpha-down-alt"></i>}
                {enableFeatures && !column.getIsSorted() && column.getCanSort() && <i className="bi bi-arrow-down-up ms-2"></i>}
                </div>
            ), 
            accessorKey: "tip", 
            enableSorting: true 
        },
        
        { 
            header: ({ column }) => (
                <div style={{ display: "flex", alignItems: "center", gap:"5px" }}>
                Datum održavanja
                {enableFeatures && column.getIsSorted() === "asc" && <i className="bi bi-sort-numeric-down ms-2"></i>}
                {enableFeatures && column.getIsSorted() === "desc" && <i class="bi bi-sort-numeric-down-alt"></i>}
                {enableFeatures && !column.getIsSorted() && column.getCanSort() && <i className="bi bi-arrow-down-up ms-2"></i>}
                </div>
            ),
            accessorKey: "datum",
            cell: info => parseDate(info.getValue()),
            enableSorting: true
        },

        { 
            header: ({ column }) => (
                <div style={{ display: "flex", alignItems: "center", gap:"5px" }}>
                Vreme početka
                {enableFeatures && column.getIsSorted() === "asc" && <i className="bi bi-sort-numeric-down ms-2"></i>}
                {enableFeatures && column.getIsSorted() === "desc" && <i class="bi bi-sort-numeric-down-alt"></i>}
                {enableFeatures && !column.getIsSorted() && column.getCanSort() && <i className="bi bi-arrow-down-up ms-2"></i>}
                </div>
            ),
            accessorKey: "vremePoc",
            cell: info => parseTime(info.row.original.datum, info.getValue()),
            enableSorting: true
        },

        { 
            header: ({ column }) => (
                <div style={{ display: "flex", alignItems: "center", gap:"5px" }}>
                Vreme kraja
                {enableFeatures && column.getIsSorted() === "asc" && <i className="bi bi-sort-numeric-down ms-2"></i>}
                {enableFeatures && column.getIsSorted() === "desc" && <i class="bi bi-sort-numeric-down-alt"></i>}
                {enableFeatures && !column.getIsSorted() && column.getCanSort() && <i className="bi bi-arrow-down-up ms-2"></i>}
                </div>
            ),
            accessorKey: "vremeKraja",
            cell: info => parseTime(info.row.original.datum, info.getValue()),
            enableSorting: true
        },

        {
            header: "Status",
            accessorKey: "status",
            enableSorting: false,
            cell: info => {
                const status = info.getValue();
                const color =
                    status === "active"  ? "#d4edda" :     
                    status === "planned" ? "#fff3cd" :      
                    status === "fading"  ? "#f8d7da" : 
                    status === "next"    ? "#d1ecf1" :
                    status === "finished"  ? "#e2e3e5" :  
                    "white";
                const naziv =
                    status === "active"  ? "ACTIVE" :     
                    status === "planned" ? "PLANNED" :      
                    status === "fading"  ? "FADING" : 
                    status === "next"  ? "NEXT" : 
                    status === "finished"  ? "FINISHED" :     
                    "white";

                return (
                    <div
                        style={{
                            backgroundColor: color,
                            padding: "2px",
                            borderRadius:"5px"
                        }}
                    >
                        {naziv}
                    </div>
                );
            }
        }];

        const hasPlanned = tableData.some(row => row.status === "planned");
        if (hasPlanned) {
            baseCols.push(
            {
                header: ({ column }) => <div><FontAwesomeIcon icon={faGear} /></div>,
                accessorKey: "edit",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnEdit"
                    onClick={() => onEditClicked(info.row.original.id)}>
                    <FontAwesomeIcon icon={faPenToSquare} />
                </button>,
                enableSorting: false
            },
            {
                header: () => <div>Označi kao sledeću</div>,
                accessorKey: "setNext",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnPromote btnNext"
                    onClick={() => onSetNext(info.row.original.id)}
                >
                    <FontAwesomeIcon icon={faCircleUp} />
                </button>,
                enableSorting: false
            }
        );
        }

        const hasNext = tableData.some(row => row.status === "next");
        if (hasNext) {
            baseCols.push(
            {
                header: ({ column }) => <div>Označi kao aktivnu</div>,
                accessorKey: "setActive",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnPromote btnActive"
                    onClick={() => onSetActive(info.row.original.id)}>
                    <FontAwesomeIcon icon={faCircleUp} />
                </button>,
                enableSorting: false
            },
            {
                header: () => <div>Vrati u planirane</div>,
                accessorKey: "setPlanned",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnPromote btnPlanned"
                    onClick={() => onSetPlanned(info.row.original.id)}
                >
                    <FontAwesomeIcon icon={faCircleDown} />
                </button>,
                enableSorting: false
            }
        );
        }

        const hasActiveOrFading = tableData.some(row => row.status === "active" || row.status === "fading");
        if (hasActiveOrFading) {
            baseCols.push(
            {
                header: ({ column }) => <div>Upravljaj resursima</div>,
                accessorKey: "layout",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnPromote"
                    onClick={() => onViewResources(info.row.original.id)}>
                    <FontAwesomeIcon icon={faEye} />
                </button>,
                enableSorting: false
            },
        );
        }

        const hasFinished = tableData.some(row => row.status === "finished");
        if (hasFinished) {
            baseCols.push(
            {
                header: ({ column }) => <div>Kloniraj u planiranu</div>,
                accessorKey: "cloneToPlanned",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnEdit"
                    onClick={() => cloneFinishedSession(info.row.original.id)}>
                    <FontAwesomeIcon icon={faClone} />
                </button>,
                enableSorting: false
            },
            {
                header: () => <div>Obriši</div>,
                accessorKey: "deleteFinished",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnEdit"
                    onClick={() => onSessionDeleted(info.row.original.id)}
                >
                    <FontAwesomeIcon icon={faTrashCan} />
                </button>,
                enableSorting: false
            }
        );
        }

        return baseCols;
    }, [tableData]);

    const table = useReactTable({
        data: tableData,
        columns,
        state: enableFeatures
        ? { pagination: { pageIndex, pageSize }, sorting, globalFilter }
        : {},
        onPaginationChange: enableFeatures
            ? (updater) => {
                const newState = typeof updater === "function"
                    ? updater({ pageIndex, pageSize })
                    : updater;
                setPageIndex(newState.pageIndex);
                setPageSize(newState.pageSize);
            }
            : undefined,
        onSortingChange: enableFeatures ? setSorting : undefined,
        onGlobalFilterChange: enableFeatures ? setGlobalFilter : undefined,
        getCoreRowModel: getCoreRowModel(),
        getPaginationRowModel: enableFeatures ? getPaginationRowModel() : undefined,
        getSortedRowModel: enableFeatures ? getSortedRowModel() : undefined,
        getFilteredRowModel: enableFeatures ? getFilteredRowModel() : undefined,
    });

    const cloneFinishedSession = async (id) =>
    {
        //fetch za clone

        const finishedSession = tableData.find(s => s.id == id);
        const clonedSession = new SessionView(finishedSession.id + 1000, finishedSession.naziv, finishedSession.tip,
            finishedSession.datum, finishedSession.vremePoc, finishedSession.vremeKraja, 
            0, finishedSession.autoStart, finishedSession.autoEnd, finishedSession.autoState);

        console.log(finishedSession);
        console.log(clonedSession);

        onSessionCloned(id);
    }

    

    return (
        <div className="container mt-4 tabelaDiv">
            <div className="tabelaIznad">
                <h4>{naslov}</h4>
                {enableFeatures && (
                    <div className="input-group mb-3" style={{display:"flex", gap:"5px", alignItems:"center"}}>
                        <span className="input-group-text" id="search-addon">
                            <FontAwesomeIcon icon={faMagnifyingGlass} />
                        </span>
                        <input
                        type="text"
                        className="form-control search-box"
                        value={globalFilter ?? ""}
                        onChange={(e) => setGlobalFilter(e.target.value)}
                        aria-label="Pretraga"
                        aria-describedby="search-addon"
                        />
                    </div>
                    )}
            </div>
            <table className="table table-bordered table-striped tabela">
                <thead>
                {table.getHeaderGroups().map(headerGroup => (
                    <tr key={headerGroup.id}>
                    {headerGroup.headers.map(header => (
                        <th
                            key={header.id}
                            onClick={header.column.getToggleSortingHandler()}
                            style={{ cursor: "pointer", userSelect: "none" }}
                            >
                            {flexRender(header.column.columnDef.header, header.getContext())}
                        </th>
                    ))}
                    </tr>
                ))}
                </thead>

                <tbody>
                {table.getRowModel().rows.map(row => (
                    <tr 
                        key={row.id} 
                        className={row.original.id === newlyAddedId ? "highlight-row shake-row" : ""}
                        onClick={() => row.toggleSelected()}
                        onAnimationEnd={() => {
                            if (row.original.id === newlyAddedId) {
                            setNewlyAddedId(null);
                            }
                        }}
                    >
                        {row.getVisibleCells().map(cell => 
                        (
                            <td key={cell.id}>
                            {flexRender(
                                cell.column.columnDef.cell ?? cell.column.columnDef.accessorKey,
                                cell.getContext()
                            )}
                            </td>
                        ))}
                    </tr>
                ))}
                </tbody>
            </table>
            {enableFeatures && ( <div className="d-flex justify-content-between align-items-center mt-3 paginacija">
                <div className="btn-group">
                    <button 
                        className="btn btn-sm btn-primary" 
                        onClick={() => table.setPageIndex(0)} 
                        disabled={!table.getCanPreviousPage()}
                        title="Prva stranica"
                    >
                    <i class="bi bi-chevron-double-left"></i>
                    </button>
                    <button 
                        className="btn btn-sm btn-primary" 
                        onClick={() => table.previousPage()} 
                        disabled={!table.getCanPreviousPage()}
                        title="Prethodna"
                    >
                    <i className="bi bi-chevron-left"></i>
                    </button>
                    <button 
                        className="btn btn-sm btn-primary" 
                        onClick={() => table.nextPage()} 
                        disabled={!table.getCanNextPage()}
                        title="Sledeća"
                    >
                    <i className="bi bi-chevron-right"></i>
                    </button>
                    <button 
                        className="btn btn-sm btn-primary" 
                        onClick={() => table.setPageIndex(table.getPageCount() - 1)} 
                        disabled={!table.getCanNextPage()}
                        title="Poslednja stranica"
                    >
                    <i class="bi bi-chevron-double-right"></i>
                    </button>
                </div>

                <div className="stranice">
                    {table.getPageCount() === 0 
                    ? "0 / 0" 
                    : `${table.getState().pagination.pageIndex + 1} / ${table.getPageCount()}`}
                </div>
            </div> )}
        </div>
    );
}
