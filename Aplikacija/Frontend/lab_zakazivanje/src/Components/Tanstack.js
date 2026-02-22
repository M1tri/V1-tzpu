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
import { faMagnifyingGlass, faPenToSquare, faGear } from '@fortawesome/free-solid-svg-icons';

export default function Tanstack({tableData, naslov, enableFeatures = true, newlyAddedId, setNewlyAddedId, onEditClicked})
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
            baseCols.push({
                header: ({ column }) => <div><FontAwesomeIcon icon={faGear} /></div>,
                accessorKey: "edit",
                cell: info => 
                <button 
                    className="btn btn-sm btn-warning btnEdit"
                    onClick={() => onEditClicked(info.row.original.id)}>
                    <FontAwesomeIcon icon={faPenToSquare} />
                </button>,
                enableSorting: false
            });
        }

        return baseCols;
    }, [tableData]);

    function parseDate(datum)
    {
        let date = new Date(datum);

        let dan = String(date.getDate()).padStart(2, '0');
        let mesec = String(date.getMonth()+1).padStart(2,'0');
        let godina = date.getFullYear();

        let formatirano = `${dan}. ${mesec}. ${godina}.`;
        return formatirano;
    }

    function parseTime(datum, vreme)
    {
        let time = new Date(`${datum}T${vreme}`); 

        let hh = String(time.getHours()).padStart(2, '0');
        let mm = String(time.getMinutes()).padStart(2, '0');

        let formatirano = `${hh}:${mm}`;

        return formatirano;
    }

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
