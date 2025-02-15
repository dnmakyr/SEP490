<template>
    <div class="h-full overflow-y-scroll p-1">
      <div>
        <div class="flex justify-between space-x-4 pb-4">
          <div class="flex space-x-2">
            <div class="flex h-10 w-full border border-input px-3 py-2 file:border-0 max-w-sm items-center">
              <Skeleton class="w-[8rem] max-w-full" />
            </div>
            <div class="flex h-10 items-center justify-between border border-input px-3 py-2 [&amp;>span]:line-clamp-1 w-[20rem]">
              <span>
                <Skeleton class="w-[7.5rem] max-w-full" />
              </span>
              <SVGSkeleton class="w-[1.5rem] h-[1.5rem]" />
            </div>
          </div>
          <div class="inline-flex items-center justify-center transition-colors border border-input h-10 px-4 py-2">
            <Skeleton class="w-[7.5rem] max-w-full" />
          </div>
        </div>
        <div class="border">
          <div class="relative w-full overflow-auto">
            <table class="w-full caption-bottom">
              <thead class="[&amp;_tr]:border-b">
                <tr class="border-b transition-colors">
                  <th class="h-12 px-4 text-left align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <Skeleton class="w-[0.875rem] max-w-full" />
                  </th>
                  <th class="h-12 px-4 text-left align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <Skeleton class="w-[2rem] max-w-full" />
                  </th>
                  <th class="h-12 px-4 text-left align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <Skeleton class="w-[3rem] max-w-full" />
                  </th>
                  <th class="h-12 px-4 text-left align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <Skeleton class="w-[5rem] max-w-full" />
                  </th>
                </tr>
              </thead>
              <tbody class="[&amp;_tr:last-child]:border-0">
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[0.875rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
                <tr class="border-b transition-colors">
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[1rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[4.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5.5rem] max-w-full" />
                    </div>
                  </td>
                  <td class="p-4 align-middle [&amp;:has([role=checkbox])]:pr-0">
                    <div>
                      <Skeleton class="w-[5rem] max-w-full" />
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <div class="flex items-center justify-end py-4 space-x-2">
          <div class="inline-flex items-center justify-center transition-colors border border-input h-9 px-3">
            <Skeleton class="w-[5rem] max-w-full" />
          </div>
          <div class="inline-flex items-center justify-center transition-colors border border-input h-9 px-3">
            <Skeleton class="w-[3rem] max-w-full" />
          </div>
        </div>
      </div>
    </div>
</template>